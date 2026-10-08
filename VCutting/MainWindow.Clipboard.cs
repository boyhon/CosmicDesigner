using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;

namespace VCutting;
public partial class MainWindow
{
    string? _lastPasteData;int _pasteStep;
    DesignObject? ClipboardSelection=>FlatView.SelectedObject??(ObjectTree.SelectedItem as TreeViewItem)?.Tag as DesignObject;
    static bool TextClipboardFocus=>Keyboard.FocusedElement is TextBoxBase or PasswordBox;
    bool HandleShapeClipboard(KeyEventArgs e)
    {
        if(Keyboard.Modifiers!=ModifierKeys.Control||e.Key is not (Key.C or Key.V)||TextClipboardFocus)return false;
        if(e.Key==Key.C)CopyShape_Click(this,e);else PasteShape_Click(this,e);return true;
    }
    void CopyShape_Click(object sender,RoutedEventArgs e)
    {
        if(Keyboard.FocusedElement is TextBoxBase box){box.Copy();return;}
        if(!ShapeClipboard.TryCapture(_document,ClipboardSelection,out var text)){StatusText.Text=Localization.Text("clipboard.unsupported");return;}
        try
        {
            var data=new DataObject();data.SetData(ShapeClipboard.Format,text);Clipboard.SetDataObject(data,true);
            _lastPasteData=null;_pasteStep=0;StatusText.Text=Localization.Text("clipboard.copied");
        }
        catch(ExternalException){StatusText.Text=Localization.Text("clipboard.busy");}
    }
    static string? ReadShapeClipboard()=>Clipboard.GetData(ShapeClipboard.Format) as string;
    void PasteShape_Click(object sender,RoutedEventArgs e)
    {
        if(Keyboard.FocusedElement is TextBoxBase box){box.Paste();return;}
        try
        {
            var text=ReadShapeClipboard();var step=text==_lastPasteData?_pasteStep+1:1;
            if(!ShapeClipboard.TryPrepare(_document,text,step,out var prepared,out var joints)||prepared is null){StatusText.Text=Localization.Text("clipboard.invalid");return;}
            RecordEdit();var selected=ShapeClipboard.Insert(_document,prepared,joints);_lastPasteData=text;_pasteStep=step;
            FlatView.SelectedObject=selected;WSection.SelectedObject=selected;HSection.SelectedObject=selected;
            ActivateSelectMode(Localization.Text("clipboard.pasted"));RefreshAll();SelectTreeObject(selected.Id);ShowProperties(selected);UpdateHistoryUi();FlatView.Focus();
        }
        catch(ExternalException){StatusText.Text=Localization.Text("clipboard.busy");}
    }
    void EditMenu_Opened(object sender,RoutedEventArgs e)
    {
        if(e.OriginalSource!=sender)return;
        CopyShapeMenu.IsEnabled=TextClipboardFocus||ShapeClipboard.TryCapture(_document,ClipboardSelection,out _);
        try{PasteShapeMenu.IsEnabled=TextClipboardFocus||ShapeClipboard.TryPrepare(_document,ReadShapeClipboard(),1,out _,out _);}
        catch(ExternalException){PasteShapeMenu.IsEnabled=false;}
    }
}
