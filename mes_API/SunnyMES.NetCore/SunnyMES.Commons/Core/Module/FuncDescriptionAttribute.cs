using System.ComponentModel;

namespace SunnyMES.Commons.Module;

public class FuncDescriptionAttribute: DescriptionAttribute
{
    public FuncDescriptionAttribute(string description)
        : base(description)
    {

    }
    public FuncDescriptionAttribute(string description, string funcName)
        : base(description)
    {
        this.FuncName = funcName;
    }
    

    public string FuncName { get; set; }
}