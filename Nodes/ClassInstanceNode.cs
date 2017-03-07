﻿using System;
using System.Drawing;
using ReClassNET.UI;
using ReClassNET.Util;

namespace ReClassNET.Nodes
{
	public class ClassInstanceNode : BaseReferenceNode
	{
		/// <summary>Size of the node in bytes.</summary>
		public override int MemorySize => InnerNode.MemorySize;

		public override bool PerformCycleCheck => true;

		public override void Intialize()
		{
			InnerNode = ClassNode.Create();
			InnerNode.Intialize();
		}

		/// <summary>Draws this node.</summary>
		/// <param name="view">The view information.</param>
		/// <param name="x">The x coordinate.</param>
		/// <param name="y">The y coordinate.</param>
		/// <returns>The height the node occupies.</returns>
		public override Size Draw(ViewInfo view, int x, int y)
		{
			if (IsHidden)
			{
				return DrawHidden(view, x, y);
			}

			AddSelection(view, x, y, view.Font.Height);

			x = AddOpenClose(view, x, y);
			x = AddIcon(view, x, y, Icons.Class, -1, HotSpotType.None);

			var tx = x;
			x = AddAddressOffset(view, x, y);

			x = AddText(view, x, y, view.Settings.TypeColor, HotSpot.NoneId, "Instance") + view.Font.Width;
			x = AddText(view, x, y, view.Settings.NameColor, HotSpot.NameId, Name);
			x = AddText(view, x, y, view.Settings.ValueColor, HotSpot.NoneId, $"<{InnerNode.Name}>");
			x = AddIcon(view, x, y, Icons.Change, 4, HotSpotType.ChangeType);

			x += view.Font.Width;
			x = AddComment(view, x, y);

			y += view.Font.Height;

			if (levelsOpen[view.Level])
			{
				var v = view.Clone();
				v.Address = view.Address.Add(Offset);
				v.Memory = view.Memory.Clone();
				v.Memory.Offset = Offset.ToInt32();

				var innerSize = InnerNode.Draw(v, tx, y);
				x = Math.Max(x, innerSize.Width);
				y = innerSize.Height;
			}

			AddTypeDrop(view, y);
			AddDelete(view, y);

			return new Size(x, y);
		}

		public override Size CalculateSize(ViewInfo view)
		{
			if (IsHidden)
			{
				return HiddenSize;
			}

			var h = view.Font.Height;
			if (levelsOpen[view.Level])
			{
				h += InnerNode.CalculateSize(view).Height;
			}
			return new Size(0, h);
		}
	}
}
