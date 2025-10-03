import { Documents } from '@core/domain-classes/document';
import SparkMD5 from 'spark-md5';
import { environment } from '@environments/environment';
import * as streamSaver from 'streamsaver';
import { Porgress, Chunk } from '@core/core.types';

type progress = (info: Porgress) => void;
export const ComputeMD5 = (file: File, info: progress): Promise<string> => {
  return new Promise((resolve, rejected) => {
    let blobSlice = File.prototype.slice,
      chunkSize = 2 * 1024 * 1024,
      chunks = Math.ceil(file.size / chunkSize),
      currentChunk = 0,
      spark = new SparkMD5.ArrayBuffer(),
      fileReader = new FileReader();

    let time = new Date().getTime();

    fileReader.onload = (e) => {
      spark.append(e.target.result); // Append array buffer
      currentChunk++;
      if (currentChunk < chunks) {
        console.log(
          `第${currentChunk}分片解析完成, 开始第${
            currentChunk + 1
          } / ${chunks}分片解析`
        );
        info({ percent: Number(((currentChunk / chunks) * 100).toFixed(2)) });
        loadNext();
      } else {
        console.log('finished loading');
        let md5 = spark.end();
        console.log(
          `MD5计算完成：${file.name} \nMD5：${md5} \n分片：${chunks} 大小:${
            file.size
          } 用时：${new Date().getTime() - time} ms`
        );
        spark.destroy(); //释放缓存
        info({ percent: 100 });
        resolve(md5);
      }
    };

    fileReader.onerror = () => {
      console.warn('oops, something went wrong.');
    };

    let loadNext = () => {
      let start = currentChunk * chunkSize,
        end = start + chunkSize >= file.size ? file.size : start + chunkSize;

      fileReader.readAsArrayBuffer(blobSlice.call(file, start, end));
    };

    loadNext();
  });
};
type process = (info: Chunk) => Promise<boolean>;
export const ChunkFile = async (file: File, info: process) => {
  let blobSlice = File.prototype.slice,
    chunkSize = 1 * 1024 * 1024,
    size = file.size,
    chunks = Math.ceil(size / chunkSize);

  for (let i = 0; i < chunks; i++) {
    let start = i * chunkSize,
      end = start + chunkSize >= file.size ? file.size : start + chunkSize;
    let result = await info({
      current: i + 1,
      total: chunks,
      file: new File([blobSlice.call(file, start, end)], file.name),
      size: size,
    });
    if (!result) break;
  }
};

export const Download = (document: Documents) => {
  const isVersion = document.isVersion ? document.isVersion : false;
  let baseUrl = environment.apiUrl;
  let url = '';
  if (document.isFromPreview) {
    url = `document/${document.id}/download/token/${document.token}?isVersion=${isVersion}`;
  } else {
    url = `document/${document.id}/download?isVersion=${isVersion}`;
  }

  streamSaver.mitm = 'streamsaver/mitm.html?version=2.0.0';
  let token = localStorage.getItem('bearerToken');
  const fileStream = streamSaver.createWriteStream(document.name);
  return fetch(`${baseUrl}api/${url}`, {
    method: 'get',
    cache: 'no-cache',
    headers: {
      'Content-Type': 'application/json',
      Authorization: 'Bearer ' + token,
    },
  }).then((res) => {
    const readableStream = res.body;
    if (window.WritableStream && readableStream.pipeTo) {
      return readableStream.pipeTo(fileStream);
    }
    let _window = window as any;
    _window.writer = fileStream.getWriter();
    const reader = res.body.getReader();
    const pump = () =>
      reader
        .read()
        .then((res) =>
          res.done
            ? _window.writer.close()
            : _window.writer.write(res.value).then(pump)
        );
    pump();
  });
};
