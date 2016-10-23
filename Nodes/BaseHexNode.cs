﻿using System;
using System.Diagnostics.Contracts;
using System.Globalization;

namespace ReClassNET.Nodes
{
	abstract class BaseHexNode : BaseNode
	{
		private byte[] buffer;
		private DateTime highlightUntil;

		public static DateTime CurrentHighlightTime;
		public static TimeSpan HightlightDuration = TimeSpan.FromSeconds(1);

		public BaseHexNode()
		{
			buffer = new byte[MemorySize];
		}

		public int Draw(ViewInfo view, int x, int y, string text, int length)
		{
			Contract.Requires(view != null);
			Contract.Requires(text != null);

			if (IsHidden)
			{
				return DrawHidden(view, x, y);
			}

			AddSelection(view, x, y, view.Font.Height);
			AddDelete(view, x, y);
			AddTypeDrop(view, x, y);

			x += TXOFFSET + 16;
			x = AddAddressOffset(view, x, y);

			if (Program.Settings.ShowNodeText)
			{
				x = AddText(view, x, y, Program.Settings.TextColor, HotSpot.NoneId, text);
			}

			var color = Program.Settings.HighlightChangedValues && highlightUntil > CurrentHighlightTime ? Program.Settings.HighlightColor : Program.Settings.HexColor;
			var changed = false;
			for (var i = 0; i < length; ++i)
			{
				var b = view.Memory.ReadByte(Offset + i);
				if (buffer[i] != b)
				{
					changed = true;

					buffer[i] = b;
				}

				x = AddText(view, x, y, color, i, $"{b:X02}") + view.Font.Width;
			}

			if (changed)
			{
				highlightUntil = CurrentHighlightTime.Add(HightlightDuration);
			}

			x = AddComment(view, x, y);

			return y + view.Font.Height;
		}

		/// <summary>Updates the node from the given spot. Sets the value of the selected byte.</summary>
		/// <param name="spot">The spot.</param>
		public void Update(HotSpot spot, int length)
		{
			Contract.Requires(spot != null);

			base.Update(spot);

			if (spot.Id >= 0 && spot.Id < length)
			{
				byte val;
				if (byte.TryParse(spot.Text, NumberStyles.HexNumber, null, out val))
				{
					spot.Memory.Process.WriteRemoteMemory(spot.Address + spot.Id, val);
				}
			}
		}
	}
}
