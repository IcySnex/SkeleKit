namespace SkeleKit.Gallery.Views.Showcase;

internal static class CSharpSyntax
{
	enum TokenKind
	{
		Plain,
		Keyword,
		Symbol,
		String,
		Number,
		Comment
	}

	static readonly Color KeywordColor = Color.Dynamic(
		Color.FromHex(0x9B2393),
		Color.FromHex(0xFC5FA3));

	static readonly Color SymbolColor = Color.Dynamic(
		Color.FromHex(0x0B4F79),
		Color.FromHex(0x5DD8FF));

	static readonly Color StringColor = Color.Dynamic(
		Color.FromHex(0xC41A16),
		Color.FromHex(0xFC6A5D));

	static readonly Color NumberColor = Color.Dynamic(
		Color.FromHex(0x1C00CF),
		Color.FromHex(0xD0BF69));

	static readonly HashSet<string> Keywords = new(StringComparer.Ordinal)
	{
		"abstract", "as", "async", "await", "base", "bool", "break", "byte",
		"case", "catch", "char", "checked", "class", "const", "continue",
		"decimal", "default", "delegate", "do", "double", "else", "enum",
		"event", "explicit", "extern", "false", "field", "file", "finally",
		"fixed", "float", "for", "foreach", "get", "global", "goto", "if",
		"implicit", "in", "init", "int", "interface", "internal", "is", "lock",
		"long", "namespace", "new", "not", "null", "object", "operator", "or",
		"out", "override", "params", "partial", "private", "protected", "public",
		"readonly", "record", "ref", "required", "return", "sbyte", "sealed",
		"set", "short", "sizeof", "stackalloc", "static", "string", "struct",
		"switch", "this", "throw", "true", "try", "typeof", "uint", "ulong",
		"unchecked", "unsafe", "ushort", "using", "var", "virtual", "void",
		"volatile", "when", "where", "while", "with", "yield"
	};


	public static IReadOnlyList<Span> Highlight(
		IReadOnlyList<Span> source)
	{
		string code = string.Concat(source.Select(span => span.Text));
		List<Span> result = [];
		List<TokenKind> kinds = [];

		int index = 0;
		while (index < code.Length)
		{
			int start = index;

			if (code[index] == '/' && index + 1 < code.Length && code[index + 1] == '/')
			{
				index += 2;
				while (index < code.Length && code[index] != '\n')
					index++;

				Add(result, kinds, code[start..index], TokenKind.Comment);
				continue;
			}

			if (code[index] == '/' && index + 1 < code.Length && code[index + 1] == '*')
			{
				index += 2;
				while (index + 1 < code.Length && (code[index] != '*' || code[index + 1] != '/'))
					index++;

				index = Math.Min(index + 2, code.Length);
				Add(result, kinds, code[start..index], TokenKind.Comment);
				continue;
			}

			if (TryString(code, index, out int stringEnd))
			{
				index = stringEnd;
				Add(result, kinds, code[start..index], TokenKind.String);
				continue;
			}

			if (code[index] == '\'')
			{
				index = CharacterEnd(code, index);
				Add(result, kinds, code[start..index], TokenKind.String);
				continue;
			}

			if (char.IsDigit(code[index]))
			{
				index++;
				while (index < code.Length && IsNumberPart(code[index]))
					index++;

				Add(result, kinds, code[start..index], TokenKind.Number);
				continue;
			}

			if (IsIdentifierStart(code[index]))
			{
				index++;
				while (index < code.Length && IsIdentifierPart(code[index]))
					index++;

				string identifier = code[start..index];
				TokenKind kind = Keywords.Contains(identifier)
					? TokenKind.Keyword
					: char.IsUpper(identifier[0]) ? TokenKind.Symbol : TokenKind.Plain;

				Add(result, kinds, identifier, kind);
				continue;
			}

			if (code[index] == '#' && IsLinePrefixWhitespace(code, index))
			{
				while (index < code.Length && code[index] != '\n')
					index++;

				Add(result, kinds, code[start..index], TokenKind.Keyword);
				continue;
			}

			index++;
			Add(result, kinds, code[start..index], TokenKind.Plain);
		}

		return result;
	}


	static void Add(
		List<Span> spans,
		List<TokenKind> kinds,
		string text,
		TokenKind kind)
	{
		if (text.Length == 0)
			return;

		if (kinds.Count > 0 && kinds[^1] == kind)
		{
			spans[^1].Text += text;
			return;
		}

		spans.Add(new Span(text)
		{
			TextColor = kind switch
			{
				TokenKind.Keyword => KeywordColor,
				TokenKind.Symbol => SymbolColor,
				TokenKind.String => StringColor,
				TokenKind.Number => NumberColor,
				TokenKind.Comment => Colors.SecondaryLabel,
				_ => null
			}
		});
		kinds.Add(kind);
	}

	static bool TryString(
		string code,
		int start,
		out int end)
	{
		int index = start;
		bool verbatim = false;

		if (code[index] == '@')
		{
			verbatim = true;
			index++;
			if (index < code.Length && code[index] == '$')
				index++;
		}
		else
		{
			while (index < code.Length && code[index] == '$')
				index++;

			if (index < code.Length && code[index] == '@')
			{
				verbatim = true;
				index++;
			}
		}

		if (index >= code.Length || code[index] != '"')
		{
			end = start;
			return false;
		}

		int quoteCount = 0;
		while (index + quoteCount < code.Length && code[index + quoteCount] == '"')
			quoteCount++;

		if (quoteCount >= 3)
		{
			index += quoteCount;
			while (index < code.Length)
			{
				int closing = 0;
				while (index + closing < code.Length && code[index + closing] == '"')
					closing++;

				if (closing >= quoteCount)
				{
					end = index + quoteCount;
					return true;
				}

				index += Math.Max(closing, 1);
			}

			end = code.Length;
			return true;
		}

		index++;
		while (index < code.Length)
		{
			if (!verbatim && code[index] == '\\')
			{
				index = Math.Min(index + 2, code.Length);
				continue;
			}

			if (code[index] == '"')
			{
				if (verbatim && index + 1 < code.Length && code[index + 1] == '"')
				{
					index += 2;
					continue;
				}

				end = index + 1;
				return true;
			}

			index++;
		}

		end = code.Length;
		return true;
	}

	static int CharacterEnd(
		string code,
		int start)
	{
		int index = start + 1;
		while (index < code.Length)
		{
			if (code[index] == '\\')
			{
				index = Math.Min(index + 2, code.Length);
				continue;
			}

			if (code[index] == '\'')
				return index + 1;

			index++;
		}

		return code.Length;
	}

	static bool IsLinePrefixWhitespace(
		string code,
		int index)
	{
		for (int current = index - 1; current >= 0 && code[current] != '\n'; current--)
		{
			if (!char.IsWhiteSpace(code[current]))
				return false;
		}

		return true;
	}

	static bool IsIdentifierStart(
		char value) =>
		value == '_' || char.IsLetter(value);

	static bool IsIdentifierPart(
		char value) =>
		value == '_' || char.IsLetterOrDigit(value);

	static bool IsNumberPart(
		char value) =>
		char.IsLetterOrDigit(value) || value is '_' or '.';
}
