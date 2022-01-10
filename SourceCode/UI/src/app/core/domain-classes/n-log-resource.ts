import { ResourceParameter } from './resource-parameter';

export class NLogResource extends ResourceParameter {
    message?: string = '';
    level?: string = 'Fatal';
    source?: string = '.Net Core';
}