using System.IO;
using System.Windows;
namespace CosmicConvert;
public static class Program
{
    [STAThread] public static int Main(string[] args)
    {
        if(args.Length!=0)return RunQuiet(args);
        var app=new Application();return app.Run(new MainWindow());
    }
    public static int RunQuiet(string[] args)
    {
        try
        {
            if(args.Length!=1||string.IsNullOrWhiteSpace(args[0]))throw new ConversionException(2,"Expected one extensionless file path.");
            var source=args[0]+".svg";var destination=args[0]+".dxf";
            var drawing=SvgReader.Read(source);DxfWriter.Save(Optimizer.Convert(drawing),destination);return 0;
        }
        catch(ConversionException ex){Diagnostic(ex.Message);return ex.Code;}
        catch(Exception ex){Diagnostic(ex.Message);return 1;}
    }
    static void Diagnostic(string text){try{Console.Error.WriteLine(text);}catch(IOException){}}
}
