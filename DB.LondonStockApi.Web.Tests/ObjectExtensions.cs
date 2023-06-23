namespace DB.LondonStockApi.Web.Tests;

public static class ObjectExtensions
{
    /// <summary>
    /// This method can be used to set an `init` property at runtime. It is only to faciltate testing by replicating the
    /// action of the Entity Framework context. DO NOT copy and use this method in actual code, it is for testing
    /// puposes ONLY.
    /// </summary>
    public static void SetAtRuntime<TObject, TPropety>(this TObject obj, Expression<Func<TObject, TPropety>> getPropety, TPropety value)
    {
        if (getPropety is not LambdaExpression lambda ||
            lambda.Body is not MemberExpression member ||
            member.Member is not PropertyInfo property)
        {
            throw new ArgumentException("getPropety must be a lambda expression that returns a property", nameof(getPropety));
        }
        property.SetValue(obj, value);
    }
}
