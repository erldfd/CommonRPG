using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace CommonRPG
{
    /// <summary>
    /// init plaease at first use of this class
    /// </summary>
    public class ListView : MonoBehaviour
    {
        [SerializeField]
        private RectTransform viewportRectTransform;

        //[SerializeField]
        //private RectTransform contentRectTransform;

        [SerializeField]
        private SPadding viewportPaddng;

        [SerializeField]
        private GameObject entryObject;

        [SerializeField]
        private SPadding entryPadding;

        [SerializeField]
        private ScrollRect listViewScrollRect;

        [SerializeField]
        private bool isHorizontal = false;

        //private LinkedList<IListViewEntry> entries = new();
        //private LinkedList<RectTransform> entryRectTransforms = new();
        private LinkedList<ListViewEntryData> entryData = new();
        public LinkedList<ListViewEntryData> EntryData { get { return entryData; } }

        private List<ListViewItem> listViewItemList = new();

        protected virtual void Awake()
        {
            //entryQueue.last
            listViewScrollRect.vertical = true;
            listViewScrollRect.horizontal = false;

            //for (int i = 0; i < 10; ++i)
            //{
            //    AddItem(new QuestWindowItem());
            //}

            //Init();
        }

        protected virtual void Start()
        {
            
        }

        protected void OnEnable()
        {
            listViewScrollRect.onValueChanged.AddListener(OnScroll);
        }

        protected void OnDisable()
        {
            listViewScrollRect.onValueChanged.RemoveListener(OnScroll);
        }

        public void AddItem(ListViewItem item)
        {
            listViewItemList.Add(item);

            int listViewItemListCount = listViewItemList.Count;

            item.Index = listViewItemListCount - 1;

            float totalListViewSize = viewportRectTransform.rect.height - viewportPaddng.top - viewportPaddng.bottom;
            float entrySize = entryObject.GetComponent<RectTransform>().rect.height + entryPadding.top + entryPadding.bottom;

            int entryMaxCount = (int)(totalListViewSize / entrySize) + 2;

            if (listViewItemListCount <= entryMaxCount) 
            {
                GameObject newEntry = Instantiate(entryObject, viewportRectTransform.transform);
                RectTransform entryRectTransform = newEntry.GetComponent<RectTransform>();

                entryRectTransform.anchorMin = new Vector2(0f, 1.0f);
                entryRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                entryRectTransform.pivot = new Vector2(0.5f, 1.0f);

                entryRectTransform.anchoredPosition = new Vector2(0, -viewportPaddng.top - entryPadding.top - entrySize * item.Index);

                IListViewEntry listViewEntry = newEntry.GetComponent<IListViewEntry>();
                entryData.AddLast(new ListViewEntryData(listViewEntry, entryRectTransform, item.Index));
            }

            entryData.Last.Value.Entry.OnUpdateEntry(item);

            OnScroll(Vector2.zero); // for refresh
        }

        //public void DeleteAllItem()
        //{
        //    listViewItemList.Clear();
        //}
        /// <summary>
        /// init plaease at first use of this class
        /// </summary>
        public void Init()
        {
            int listViewItemCount = listViewItemList.Count;

            if (listViewItemCount == 0)
            {
                return;
            }

            float totalListViewSize = viewportRectTransform.rect.height - viewportPaddng.top - viewportPaddng.bottom;
            float entrySize = entryObject.GetComponent<RectTransform>().rect.height + entryPadding.top + entryPadding.bottom;

            int entryCount = (int)(totalListViewSize / entrySize) + 2;

            if (entryCount > listViewItemCount)
            {
                entryCount = listViewItemCount;
            }

            if (entryCount == 0)
            {
                return;
            }

            for (int i = 0; i < entryCount; ++i)
            {
                GameObject newEntry = Instantiate(entryObject, viewportRectTransform.transform);
                RectTransform entryRectTransform = newEntry.GetComponent<RectTransform>();

                entryRectTransform.anchorMin = new Vector2(0f, 1.0f);
                entryRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
                entryRectTransform.pivot = new Vector2(0.5f, 1.0f);

                entryRectTransform.anchoredPosition = new Vector2(0, -viewportPaddng.top - entryPadding.top - entrySize * i);

                IListViewEntry listViewEntry = newEntry.GetComponent<IListViewEntry>();
                entryData.AddLast(new ListViewEntryData(listViewEntry, entryRectTransform, i));

                listViewEntry.OnUpdateEntry(listViewItemList[i]);
            }
        }


        private void OnScroll(Vector2 scrollPosDelta)
        {
            if (entryData.Count == 0) 
            {
                return;
            }

            float verticalDelta = scrollPosDelta.y;
            //Debug.Log($"delta : {verticalDelta}");
            //if (Mathf.Abs(verticalDelta) < 0.01f)
            //{
            //    return;
            //}

            ListViewEntryData firstNode = entryData.First.Value;
            ListViewEntryData lastNode = entryData.Last.Value;

            RectTransform firstNodeEntryRectTransform = firstNode.EntryRectTransform;
            RectTransform lastNodeEntryRectTransform = lastNode.EntryRectTransform;

            float entrySize = firstNodeEntryRectTransform.rect.height + entryPadding.top + entryPadding.bottom;

            int i = 0;
            int lastIndex = entryData.Count - 1;
            float bottomPosition = -viewportRectTransform.rect.height + viewportPaddng.bottom;

            float newPositionY = firstNodeEntryRectTransform.anchoredPosition.y - verticalDelta;

            if (verticalDelta > 0 && firstNode.EntryIndex == 0 && newPositionY <= -viewportPaddng.top - entryPadding.top)
            {
                foreach (ListViewEntryData entry in entryData)
                {
                    entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, -viewportPaddng.top - entryPadding.top - entrySize * i);
                    i++;
                }

                return;
            }

            newPositionY = lastNodeEntryRectTransform.anchoredPosition.y - verticalDelta;

            if (verticalDelta < 0 && lastNode.EntryIndex == listViewItemList.Count - 1 && newPositionY >= bottomPosition + entryPadding.bottom + lastNodeEntryRectTransform.rect.height)
            {
                foreach (ListViewEntryData entry in entryData)
                {
                    entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, bottomPosition + entryPadding.bottom + lastNodeEntryRectTransform.rect.height + entrySize * (lastIndex - i));
                    i++;
                }

                return;
            }

            foreach (ListViewEntryData entry in entryData)
            {
                entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, entry.EntryRectTransform.anchoredPosition.y - verticalDelta);
                i++;
            }

            if (verticalDelta < 0)
            {
                if (lastNode.EntryIndex != listViewItemList.Count - 1 && firstNodeEntryRectTransform.anchoredPosition.y > viewportPaddng.top + entrySize)
                {
                    firstNodeEntryRectTransform.anchoredPosition = new Vector2(firstNodeEntryRectTransform.anchoredPosition.x, lastNodeEntryRectTransform.anchoredPosition.y - entrySize);

                    firstNode.EntryIndex = lastNode.EntryIndex + 1;

                    ListViewItem updatingItem = listViewItemList[firstNode.EntryIndex];
                    firstNode.Entry.OnUpdateEntry(updatingItem);

                    entryData.RemoveFirst();
                    entryData.AddLast(firstNode);
                }
            }
            else
            {
                if (firstNode.EntryIndex != 0 && lastNodeEntryRectTransform.anchoredPosition.y < bottomPosition)
                {
                    lastNodeEntryRectTransform.anchoredPosition = new Vector2(lastNodeEntryRectTransform.anchoredPosition.x, firstNodeEntryRectTransform.anchoredPosition.y + entrySize);

                    lastNode.EntryIndex = firstNode.EntryIndex - 1;

                    ListViewItem updatingItem = listViewItemList[lastNode.EntryIndex];
                    lastNode.Entry.OnUpdateEntry(updatingItem);

                    entryData.RemoveLast();
                    entryData.AddFirst(lastNode);
                }
            }

           // Debug.Log($"first index : {entryData.First.Value.EntryIndex}, last index : {entryData.Last.Value.EntryIndex}");
        }
    }

    [Serializable]
    public struct SPadding
    {
        public float top;
        public float bottom;
        public float left;
        public float right;
    }

    public class ListViewEntryData
    {
        protected IListViewEntry entry;
        public IListViewEntry Entry { get { return entry; } set { entry = value; } }

        protected RectTransform entryRectTransform;
        public RectTransform EntryRectTransform { get { return entryRectTransform; } set { entryRectTransform = value; } }

        protected int entryIndex;
        public int EntryIndex { get { return entryIndex; } set { entryIndex = value; } }

        public ListViewEntryData(IListViewEntry entry, RectTransform entryRectTransform, int entryIndex)
        {
            this.entry = entry;
            this.entryRectTransform = entryRectTransform;
            this.entryIndex = entryIndex;
            Debug.Log("ListViewEntryData created");
        }
    }

    public class ListViewItem
    {
        public int Index { get; set; }
    }


}

