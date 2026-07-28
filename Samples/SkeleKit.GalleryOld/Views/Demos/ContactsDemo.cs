using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Demos;

namespace SkeleKit.Gallery.Views.Demos;

/// <summary>
/// A Contacts-style list with a full A-Z fast-scroll index down the side.
/// </summary>
[Page]
public class ContactsDemo : ContentView<ContactsDemoViewModel>
{
	public ContactsDemo(
		ContactsDemoViewModel viewModel) : base(viewModel)
	{
		Title = "Contacts";
		TitleStyle = TitleStyle.Large;

		Content = new CollectionView<Contact, ContactGroup>
		{
			// plain, so the letter headers pin under the bar like the real Contacts app
			Layout = CollectionLayout.List(grouped: false),
			ItemTemplate = () => new ContactCell(),
			HeaderTemplate = () => new ContactHeader(),
			GroupedItemsSource = viewModel.Groups,

			// one label per section, plus the full alphabet so gap letters still show and jump ahead
			SectionIndexTitle = group => group.Letter,
			IndexTitles = viewModel.Alphabet
		};
	}
}

file class ContactCell : ItemView<Contact>
{
	public ContactCell() =>
		Content = new Label
		{
			Text = Bind(contact => contact.Name),
			TextStyle = TextStyle.Body,
			HorizontalAlignment = HorizontalAlignment.Start,
			Margin = new Thickness(16, 12)
		};
}

file class ContactHeader : ItemView<ContactGroup>
{
	public ContactHeader() =>
		Content = new Label
		{
			Style = Styles.SectionHeader,
			Text = Bind(group => group.Letter),
			HorizontalAlignment = HorizontalAlignment.Start,
			Margin = new Thickness(16, 4)
		};
}
