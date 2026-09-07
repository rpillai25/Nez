using System;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Text;

namespace Nez
{
	/// <summary>
	/// Interpolated-string handler for <see cref="Debug.Log(QuietLogHandler)"/>. When
	/// <see cref="Debug.QuietMode"/> is on the compiler-generated interpolation is skipped entirely
	/// (no string is built, nothing is boxed), so hot-path log calls cost a single bool check while
	/// a replay seek runs thousands of simulation steps per second. Call sites are unchanged: any
	/// <c>Debug.Log($"...")</c> binds to this overload automatically.
	/// </summary>
	[InterpolatedStringHandler]
	public ref struct QuietLogHandler
	{
		private readonly StringBuilder _builder;

		public QuietLogHandler(int literalLength, int formattedCount, out bool shouldAppend)
		{
			shouldAppend = !Debug.QuietMode;
			_builder = shouldAppend ? new StringBuilder(literalLength + formattedCount * 8) : null;
		}

		public void AppendLiteral(string value)
		{
			_builder.Append(value);
		}

		public void AppendFormatted<T>(T value)
		{
			_builder.Append(value?.ToString());
		}

		public void AppendFormatted<T>(T value, string format)
		{
			if (value is IFormattable formattable)
				_builder.Append(formattable.ToString(format, CultureInfo.CurrentCulture));
			else
				_builder.Append(value?.ToString());
		}

		public void AppendFormatted<T>(T value, int alignment)
		{
			AppendFormatted(value, alignment, null);
		}

		public void AppendFormatted<T>(T value, int alignment, string format)
		{
			string text = value is IFormattable formattable && format != null
				? formattable.ToString(format, CultureInfo.CurrentCulture)
				: value?.ToString() ?? string.Empty;
			int pad = Math.Abs(alignment) - text.Length;
			if (pad > 0 && alignment > 0)
				_builder.Append(' ', pad);
			_builder.Append(text);
			if (pad > 0 && alignment < 0)
				_builder.Append(' ', pad);
		}

		/// <summary>The built message, or empty when the interpolation was skipped.</summary>
		public string GetFormattedText()
		{
			return _builder != null ? _builder.ToString() : string.Empty;
		}
	}
}
