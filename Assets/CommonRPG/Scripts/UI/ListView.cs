using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using static UnityEditor.Progress;
using static UnityEngine.EventSystems.EventTrigger;

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
        private ListViewEntry entryObject;

        [SerializeField]
        private SPadding entryPadding;

        [SerializeField]
        private ScrollRect listViewScrollRect;

        [SerializeField]
        private bool isHorizontal = false;

        private LinkedList<ListViewEntry> entries = new();
        public LinkedList<ListViewEntry> Entries {  get { return entries; } }

        //private LinkedList<RectTransform> entryRectTransforms = new();
        //private LinkedList<ListViewEntryData> entryData = new();
        //public LinkedList<ListViewEntryData> EntryData { get { return entryData; } }

        private List<ListViewItem> listViewItemList = new();

        private Queue<ListViewEntry> deactivatedEntryQueue = new();

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
                CreateEntry();
            }

            entries.Last.Value.ItemIndexInEntry = item.Index;
            entries.Last.Value.OnUpdateEntry(item);
        }

        public void RemoveItem(ListViewItem item)
        {
            int removedItemIndex = item.Index;

            listViewItemList.Remove(item);

            ListViewEntry firstEntry = entries.First.Value;
            ListViewEntry lastEntry = entries.Last.Value;

            if (removedItemIndex > firstEntry.ItemIndexInEntry || removedItemIndex < lastEntry.ItemIndexInEntry) 
            {
                return;
            }

            int i = 0;
            int listViewItemListCount = listViewItemList.Count;

            if (lastEntry.ItemIndexInEntry == 0)
            {
                i = firstEntry.ItemIndexInEntry;

                LinkedListNode<ListViewEntry> entryNode = entries.First;

                while (entryNode != null)
                {
                    if (i <= removedItemIndex)
                    {
                        ListViewEntry entry = entryNode.Value;

                        if (listViewItemListCount > i)
                        {
                            entry.OnUpdateEntry(listViewItemList[i]);
                        }
                        else
                        {
                            entry.gameObject.SetActive(false);
                            deactivatedEntryQueue.Enqueue(entry);

                            if (entryNode.Next == null) 
                            {
                                entries.Remove(entryNode);
                                break;
                            }

                            entryNode = entryNode.Next;
                            entries.Remove(entryNode.Previous);
                            i++;

                            continue;
                        }
                    }

                    i++;
                    entryNode = entryNode.Next;
                }

                //foreach (var entry in entries)
                //{
                //    if (i <= removedItemIndex)
                //    {
                //        if (listViewItemListCount > i) 
                //        {
                //            entry.OnUpdateEntry(listViewItemList[i]);
                //        }
                //        else
                //        {
                //            entry.gameObject.SetActive(false);
                //            deactivatedEntryQueue.Enqueue(entry);
                //        }
                //    }

                //    i++;
                //}
            }
            else
            {
                LinkedListNode<ListViewEntry> entryNode = entries.First;

                while (entryNode != null)
                {
                    if (i >= removedItemIndex)
                    {
                        ListViewEntry entry = entryNode.Value;

                        if (listViewItemListCount > i)
                        {
                            entry.OnUpdateEntry(listViewItemList[i]);
                        }
                        else
                        {
                            entry.gameObject.SetActive(false);
                            deactivatedEntryQueue.Enqueue(entry);

                            if (entryNode.Next == null)
                            {
                                entries.Remove(entryNode);
                                break;
                            }

                            entryNode = entryNode.Next;
                            entries.Remove(entryNode.Previous);
                            i++;

                            continue;
                        }
                    }

                    i++;
                    entryNode = entryNode.Next;
                }

                //foreach (var entry in entries)
                //{
                //    if (i >= removedItemIndex )
                //    {
                //        if (listViewItemListCount > i) 
                //        {
                //            entry.OnUpdateEntry(listViewItemList[i]);
                //        }
                //        else
                //        {
                //            entry.gameObject.SetActive(false);
                //            deactivatedEntryQueue.Enqueue(entry);
                //        }
                //    }

                //    i++;
                //}
            }
        }

        public ListViewItem GetItemFromIndex(int index)
        {
            return (listViewItemList.Count > index) ? listViewItemList[index] : null;
        }

        private void AdjustProperEntryCount(int currentItemCount, int entryMaxCount)
        {
            int currentEntryCount = entries.Count;
            int neededEntryCount = entryMaxCount - currentEntryCount;

            if (currentItemCount < entryMaxCount)
            {
                neededEntryCount = currentItemCount;
            }

            if (neededEntryCount == 0) 
            {
                return;
            }

            if (neededEntryCount > 0) 
            {

            }
        }

        private void CreateEntry()
        {
            ListViewEntry newEntry;

            if (deactivatedEntryQueue.Count > 0) 
            {
                newEntry = deactivatedEntryQueue.Dequeue();
            }
            else
            {
                newEntry = Instantiate(entryObject, viewportRectTransform.transform);
                newEntry.EntryRectTransform = newEntry.GetComponent<RectTransform>();
            }

            newEntry.gameObject.SetActive(true);

            RectTransform entryRectTransform = newEntry.EntryRectTransform;

            entryRectTransform.anchorMin = new Vector2(0f, 1.0f);
            entryRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            entryRectTransform.pivot = new Vector2(0.5f, 1.0f);

            float entrySize = entryPadding.top + entryPadding.bottom + newEntry.EntryRectTransform.rect.height;

            entryRectTransform.anchoredPosition = new Vector2(0, -viewportPaddng.top - entryPadding.top - entrySize * entries.Count);

            entries.AddLast(newEntry);
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
            //int listViewItemCount = listViewItemList.Count;

            //if (listViewItemCount == 0)
            //{
            //    return;
            //}

            //float totalListViewSize = viewportRectTransform.rect.height - viewportPaddng.top - viewportPaddng.bottom;
            //float entrySize = entryObject.GetComponent<RectTransform>().rect.height + entryPadding.top + entryPadding.bottom;

            //int entryCount = (int)(totalListViewSize / entrySize) + 2;

            //if (entryCount > listViewItemCount)
            //{
            //    entryCount = listViewItemCount;
            //}

            //if (entryCount == 0)
            //{
            //    return;
            //}

            //for (int i = 0; i < entryCount; ++i)
            //{
            //    GameObject newEntry = Instantiate(entryObject, viewportRectTransform.transform);
            //    RectTransform entryRectTransform = newEntry.GetComponent<RectTransform>();

            //    entryRectTransform.anchorMin = new Vector2(0f, 1.0f);
            //    entryRectTransform.anchorMax = new Vector2(1.0f, 1.0f);
            //    entryRectTransform.pivot = new Vector2(0.5f, 1.0f);

            //    entryRectTransform.anchoredPosition = new Vector2(0, -viewportPaddng.top - entryPadding.top - entrySize * i);

            //    IListViewEntry listViewEntry = newEntry.GetComponent<IListViewEntry>();
            //    entryData.AddLast(new ListViewEntryData(listViewEntry, entryRectTransform, i));

            //    listViewEntry.OnUpdateEntry(listViewItemList[i]);
            //}
        }


        private void OnScroll(Vector2 scrollPosDelta)
        {
            if (entries.Count == 0) 
            {
                return;
            }

            float totalListViewSize = viewportRectTransform.rect.height - viewportPaddng.top - viewportPaddng.bottom;
            float entrySize = entryObject.GetComponent<RectTransform>().rect.height + entryPadding.top + entryPadding.bottom;

            int entryMaxCount = (int)(totalListViewSize / entrySize) + 2;

            if (entryMaxCount > listViewItemList.Count) 
            {
                return;
            }

            float verticalDelta = scrollPosDelta.y;
            //Debug.Log($"delta : {verticalDelta}");
            //if (Mathf.Abs(verticalDelta) < 0.01f)
            //{
            //    return;
            //}

            ListViewEntry firstNodeValue = entries.First.Value;
            ListViewEntry lastNodeValue = entries.Last.Value;

            RectTransform firstNodeEntryRectTransform = firstNodeValue.EntryRectTransform;
            RectTransform lastNodeEntryRectTransform = lastNodeValue.EntryRectTransform;

            int i = 0;
            int lastIndex = entries.Count - 1;
            float bottomPosition = -viewportRectTransform.rect.height + viewportPaddng.bottom;

            float newPositionY = firstNodeEntryRectTransform.anchoredPosition.y - verticalDelta;

            if (verticalDelta > 0 && firstNodeValue.ItemIndexInEntry == 0 && newPositionY <= -viewportPaddng.top - entryPadding.top)
            {
                foreach (ListViewEntry entry in entries)
                {
                    entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, -viewportPaddng.top - entryPadding.top - entrySize * i);
                    i++;
                }

                return;
            }

            newPositionY = lastNodeEntryRectTransform.anchoredPosition.y - verticalDelta;

            if (verticalDelta < 0 && lastNodeValue.ItemIndexInEntry == listViewItemList.Count - 1 && newPositionY >= bottomPosition + entryPadding.bottom + lastNodeEntryRectTransform.rect.height)
            {
                foreach (ListViewEntry entry in entries)
                {
                    entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, bottomPosition + entryPadding.bottom + lastNodeEntryRectTransform.rect.height + entrySize * (lastIndex - i));
                    i++;
                }

                return;
            }

            foreach (ListViewEntry entry in entries)
            {
                entry.EntryRectTransform.anchoredPosition = new Vector2(entry.EntryRectTransform.anchoredPosition.x, entry.EntryRectTransform.anchoredPosition.y - verticalDelta);
                i++;
            }

            if (verticalDelta < 0)
            {
                if (lastNodeValue.ItemIndexInEntry != listViewItemList.Count - 1 && firstNodeEntryRectTransform.anchoredPosition.y > viewportPaddng.top + entrySize)
                {
                    firstNodeEntryRectTransform.anchoredPosition = new Vector2(firstNodeEntryRectTransform.anchoredPosition.x, lastNodeEntryRectTransform.anchoredPosition.y - entrySize);

                    firstNodeValue.ItemIndexInEntry = lastNodeValue.ItemIndexInEntry + 1;

                    ListViewItem updatingItem = listViewItemList[firstNodeValue.ItemIndexInEntry];
                    firstNodeValue.OnUpdateEntry(updatingItem);

                    entries.RemoveFirst();
                    entries.AddLast(firstNodeValue);
                }
            }
            else
            {
                if (firstNodeValue.ItemIndexInEntry != 0 && lastNodeEntryRectTransform.anchoredPosition.y < bottomPosition)
                {
                    lastNodeEntryRectTransform.anchoredPosition = new Vector2(lastNodeEntryRectTransform.anchoredPosition.x, firstNodeEntryRectTransform.anchoredPosition.y + entrySize);

                    lastNodeValue.ItemIndexInEntry = firstNodeValue.ItemIndexInEntry - 1;

                    ListViewItem updatingItem = listViewItemList[lastNodeValue.ItemIndexInEntry];
                    lastNodeValue.OnUpdateEntry(updatingItem);

                    entries.RemoveLast();
                    entries.AddFirst(lastNodeValue);
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

