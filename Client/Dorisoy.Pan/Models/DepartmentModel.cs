namespace Dorisoy.Pan.Models;

public class DepartmentModel : BaseModel
{
    public Guid Id { get; set; }
    [Reactive] public string Name { get; set; }
    [Reactive] public string Code { get; set; }
    [Reactive] public int Level { get; set; }
    [Reactive] public int Sort { get; set; }
    [Reactive] public string FullPath { get; set; }
    [Reactive] public Guid? ParentId { get; set; }
    [Reactive] public DepartmentModel Parent { get; set; }
    [Reactive] public bool IsExpanded { get; set; }
    [Reactive] public ICollection<DepartmentModel> Children { get; set; } = [];
}



/// <summary>
/// 树形结构返回模型
/// </summary>
/// <typeparam name="TKey">编号类型</typeparam>
/// <typeparam name="T"></typeparam>
public class TreeResultModel<TKey, T> where T : class, new()
{
    public TKey Id { get; set; }
    public string Name { get; set; }
    public bool IsExpanded { get; set; }
    public object ParentId { get; set; }
    public object Value { get; set; }
    public object Level { get; set; }
    public T Item { get; set; }
    public List<string> Path { get; set; } = new();
    public ICollection<TreeResultModel<TKey, T>> Children { get; set; } = new List<TreeResultModel<TKey, T>>();
}

/// <summary>
/// 树形结构返回模型
/// </summary>
/// <typeparam name="T"></typeparam>
public class TreeResultModel<T> : TreeResultModel<Guid, T> where T : class, new()
{
    /// <summary>
    /// 子节点
    /// </summary>
    public new ICollection<TreeResultModel<T>> Children { get; set; } = new HashSet<TreeResultModel<T>>();
}
