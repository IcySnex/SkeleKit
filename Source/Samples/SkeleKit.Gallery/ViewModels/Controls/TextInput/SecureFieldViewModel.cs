using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SkeleKit.Gallery.Models;
using SkeleKit.Gallery.ViewModels.Showcase;

namespace SkeleKit.Gallery.ViewModels.Controls.TextInput;

internal sealed partial class SecureFieldViewModel : ShowcaseViewModel
{
	public SecureFieldViewModel()
	{
		SelectedIntent = PasswordIntents[1];
	}


	public List<ShowcaseOption<ContentKind>> PasswordIntents { get; } =
	[
		new("Current password", ContentKind.Password),
		new("New password", ContentKind.NewPassword),
		new("None", ContentKind.None)
	];


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(Strength))]
	[NotifyPropertyChangedFor(nameof(StrengthLabel))]
	[NotifyPropertyChangedFor(nameof(EntryCode))]
	string? text;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(EntryCode))]
	bool revealsEntry = true;

	[ObservableProperty]
	string submitStatus = "Press Done to submit.";

	public double Strength =>
		Math.Min(1, (Text?.Length ?? 0) / 14d);

	public string StrengthLabel =>
		(Text?.Length ?? 0) switch
		{
			0 => "No password entered",
			< 8 => "Password strength · Weak",
			< 12 => "Password strength · Good",
			_ => "Password strength · Strong"
		};

	public IReadOnlyList<Span> EntryCode =>
		Code(
			$$"""
			new SecureField
			{
				Text = Bind(vm => vm.Text)
					.TwoWay((vm, val) => vm.Text = val),
				Placeholder = "Create a password",
				LeadingIcon = ImageSource.Symbol("lock.fill"),
				RevealButton = {{Boolean(RevealsEntry)}},
				ContentKind = ContentKind.NewPassword,
				ReturnKey = ReturnKeyType.Done,
				RequiresText = true,
				SubmitCommand = viewModel.SubmitCommand
			};
			""");


	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TrailingOwner))]
	[NotifyPropertyChangedFor(nameof(IntentCode))]
	ShowcaseOption<ContentKind> selectedIntent = null!;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TrailingOwner))]
	[NotifyPropertyChangedFor(nameof(IntentCode))]
	bool showsReveal = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TrailingOwner))]
	[NotifyPropertyChangedFor(nameof(IntentCode))]
	bool showsTrailingIcon = true;

	[ObservableProperty]
	[NotifyPropertyChangedFor(nameof(TrailingOwner))]
	[NotifyPropertyChangedFor(nameof(IntentCode))]
	bool showsClearButton = true;

	public string TrailingOwner =>
		ShowsReveal
			? "Reveal button owns the trailing slot"
			: ShowsTrailingIcon
				? "Decorative icon owns the trailing slot"
				: ShowsClearButton
					? "Clear button owns the trailing slot"
					: "Trailing slot is empty";

	public IReadOnlyList<Span> IntentCode =>
		Code(
			$$"""
			new SecureField
			{
				Text = "Gallery password",
				ContentKind = ContentKind.{{SelectedIntent.Value}},
				RevealButton = {{Boolean(ShowsReveal)}},
				TrailingIcon = {{(ShowsTrailingIcon ? "ImageSource.Symbol(\"checkmark.circle.fill\")" : "null")}},
				ClearButton = {{(ShowsClearButton ? "ClearButton.WhileEditing" : "ClearButton.Never")}}
			};
			""");


	[RelayCommand]
	void SetExample() =>
		Text = "SkeleKit!2026";

	[RelayCommand]
	void ClearText() =>
		Text = null;

	[RelayCommand]
	void Submit() =>
		SubmitStatus = "Submitted";


	static IReadOnlyList<Span> Code(
		string value) =>
		[new(value)];

	static string Boolean(
		bool value) =>
		value ? "true" : "false";
}
