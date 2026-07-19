using System.Collections.ObjectModel;
using SkeleKit.Gallery.Models;

namespace SkeleKit.Gallery.Services;

/// <summary>
/// Supplies the control demos shown on the menu.
/// </summary>
public interface IDemoCatalog
{
	ObservableCollection<DemoEntry> Demos { get; }
}
