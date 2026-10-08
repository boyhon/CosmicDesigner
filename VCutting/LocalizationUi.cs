using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Globalization;
using System.Windows.Data;

namespace VCutting;

/// <summary>Live presentation bindings for programmatically created labels; never edits TextBox values or model IDs.</summary>
public static class LocalizationUi
{
    sealed class State {public Dictionary<DependencyProperty,(Localization.Token Token,string Last)> Properties=[];}
    static readonly ConditionalWeakTable<DependencyObject,State> States=new();
    static bool _started;
    public static void Start()
    {
        if(_started)return;_started=true;
        EventManager.RegisterClassHandler(typeof(FrameworkElement),FrameworkElement.LoadedEvent,new RoutedEventHandler((s,_)=>{if(s is DependencyObject d)Update(d);}));
        Localization.Changed+=Refresh;
        Publish();
    }
    public static void Refresh()
    {
        Publish();if(Application.Current is null)return;
        foreach(Window window in Application.Current.Windows){Visit(window,new());window.InvalidateVisual();}
    }
    public static void RefreshElement(DependencyObject root)=>Visit(root,new());
    static void Publish(){if(Application.Current is null)return;foreach(var key in Keys())Application.Current.Resources[key]=Localization.Text(key);}
    public static void ConfigureUnitCombo(ComboBox combo)
    {
        var factory=new FrameworkElementFactory(typeof(TextBlock));
        factory.SetBinding(TextBlock.TextProperty,new Binding{Converter=new UnitLabelConverter()});
        combo.ItemTemplate=new DataTemplate{VisualTree=factory};
    }
    static IEnumerable<string> Keys()
    {
        using var stream=typeof(Localization).Assembly.GetManifestResourceStream("CosmicDesigner.Languages.en.json");if(stream is null)return [];
        using var doc=System.Text.Json.JsonDocument.Parse(stream);return doc.RootElement.GetProperty("strings").EnumerateObject().Select(x=>x.Name).ToArray();
    }
    static void Visit(DependencyObject d,HashSet<DependencyObject> seen)
    {
        if(!seen.Add(d))return;Update(d);
        foreach(var child in LogicalTreeHelper.GetChildren(d).OfType<DependencyObject>())Visit(child,seen);
        if(d is Visual)for(int i=0;i<VisualTreeHelper.GetChildrenCount(d);i++)Visit(VisualTreeHelper.GetChild(d,i),seen);
        if(d is UIElement ui)ui.InvalidateVisual();
    }
    static void Update(DependencyObject d)
    {
        var properties=new List<DependencyProperty>();
        if(d is TextBlock)properties.Add(TextBlock.TextProperty);
        if(d is TextBox {IsReadOnly:true})properties.Add(TextBox.TextProperty);
        if(d is ContentControl)properties.Add(ContentControl.ContentProperty);
        if(d is HeaderedContentControl)properties.Add(HeaderedContentControl.HeaderProperty);
        if(d is HeaderedItemsControl)properties.Add(HeaderedItemsControl.HeaderProperty);
        if(d is FrameworkElement)properties.Add(FrameworkElement.ToolTipProperty);
        if(d is Window)properties.Add(Window.TitleProperty);
        foreach(var property in properties)
        {
            if(d.GetValue(property) is not string current)continue;
            // DynamicResource bindings are maintained by WPF, not replaced here.
            if(DependencyPropertyHelper.GetValueSource(d,property).IsExpression)
            {
                var binding=BindingOperations.GetBindingExpression(d,property);
                if(binding?.ParentBinding.Converter is UnitLabelConverter)binding.UpdateTarget();
                continue;
            }
            var state=States.GetOrCreateValue(d);
            if(!state.Properties.TryGetValue(property,out var previous)||current!=previous.Last)
            {if(!Localization.TryToken(current,out var token))continue;previous=(token,current);}
            var value=Localization.Render(previous.Token);d.SetCurrentValue(property,value);state.Properties[property]=(previous.Token,value);
        }
    }
}
public sealed class UnitLabelConverter : IValueConverter
{
    public object Convert(object value,Type targetType,object parameter,CultureInfo culture)=>value is MeasurementUnit unit?Localization.Text("unit."+unit):value;
    public object ConvertBack(object value,Type targetType,object parameter,CultureInfo culture)=>Binding.DoNothing;
}
