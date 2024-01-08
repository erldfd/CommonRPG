using CommonRPG;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CommonRPG
{
    public abstract class ListViewEntry : MonoBehaviour
    {
        protected RectTransform entryRectTransform;
        public RectTransform EntryRectTransform { get { return entryRectTransform; } set { entryRectTransform = value; } }

        protected int itemIndexInEntry;
        public int ItemIndexInEntry { get { return itemIndexInEntry; } set { itemIndexInEntry = value; } }

        public abstract void OnUpdateEntry(ListViewItem updatedData);
    }
}
