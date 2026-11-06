using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Media;

namespace VCutting;

public sealed partial class FlatDesignerView
{
    static readonly Pen BendWarningPen = new(Brushes.DarkOrange, 2);
    readonly ToolTip _bendWarningTip = new() { Placement = PlacementMode.Mouse, IsHitTestVisible = false };
    IReadOnlyList<BendContact> _bendContacts = [];
    public event EventHandler<bool>? BendRepairRequested;

    public void SetBendContacts(IReadOnlyList<BendContact> contacts)
    {
        _bendContacts = contacts;
        _bendWarningTip.IsOpen = false;
    }

    void DrawBendContacts(DrawingContext drawing)
    {
        foreach (var contact in _bendContacts)
            drawing.DrawEllipse(Brushes.Transparent, BendWarningPen, ToScreen(contact.Position), 6, 6);
    }

    BendContact? HitBendContact(Point screen)
    {
        if (_dragMode != DragMode.None || ActiveBendLayer is not null || ActiveSlitShape is not null ||
            ActiveHoleShape is not null || FilletMode || ShowRuler && (screen.X < 39 || screen.Y < 25))
            return null;

        foreach (var contact in _bendContacts)
            if ((ToScreen(contact.Position) - screen).Length <= 7) return contact;
        return null;
    }

    void UpdateBendContactTip(Point screen)
    {
        var hit = HitBendContact(screen);
        _bendWarningTip.PlacementTarget = this;
        _bendWarningTip.Content = hit is { } contact
            ? Localization.Text("bend.review." + contact.Kind) + "\n" + Localization.Text("bend.review.instructions")
            : null;
        _bendWarningTip.IsOpen = hit.HasValue;
    }

    bool OpenBendContactMenu(Point screen)
    {
        if (HitBendContact(screen) is null) return false;
        _bendWarningTip.IsOpen = false;
        var menu = new ContextMenu { PlacementTarget = this, Placement = PlacementMode.MousePoint };
        var cut = new MenuItem { Header = Localization.Text("bend.review.cut") };
        var hole = new MenuItem { Header = Localization.Text("bend.review.hole") };
        cut.Click += (_, _) => BendRepairRequested?.Invoke(this, false);
        hole.Click += (_, _) => BendRepairRequested?.Invoke(this, true);
        menu.Items.Add(cut);
        menu.Items.Add(hole);
        menu.IsOpen = true;
        return true;
    }

    public bool CancelActiveHole()
    {
        if (ActiveHoleShape is null) return false;
        EndDrag();
        HoleToolCancelled?.Invoke(this, EventArgs.Empty);
        return true;
    }
}
