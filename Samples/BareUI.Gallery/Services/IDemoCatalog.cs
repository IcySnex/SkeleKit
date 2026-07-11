using BareUI.Gallery.Models;

namespace BareUI.Gallery.Services;

/// <summary>
/// Supplies the control demos shown on the menu.
/// </summary>
public interface IDemoCatalog
{
	IReadOnlyList<DemoEntry> Demos { get; }
}
