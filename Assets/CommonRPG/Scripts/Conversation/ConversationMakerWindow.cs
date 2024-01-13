using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

namespace CommonRPG
{
    public class ConversationMakerWindow : EditorWindow
    {
        public enum ENodeType
        {
            Normal,
            Choice,
            Start,
            End
        }

        private readonly Rect conversationNameRect = new Rect(10, 10, 130, 20);
        private readonly Rect conversationNameInputFieldRect = new Rect(135, 10, 200, 20);

        private Rect backgroundRect;

        private string conversationNameInput;

        private GenericMenu backgroundMenu = null;

        private Event currentMouseEvent;

        private List<NodeWindow> nodeWindows = new();

        #region Node Design

        private const float DEFAULT_NODE_WINDOW_SIZE_X = 200;
        private const float DEFAULT_NODE_WINDOW_SIZE_Y = 200;

        private readonly Vector2 defaultNodeWindowSize = new Vector2(DEFAULT_NODE_WINDOW_SIZE_X, DEFAULT_NODE_WINDOW_SIZE_Y);

        private const float NODE_PADDING_TOP = 20;
        private const float NODE_PADDING_BOTTOM = 10;
        private const float NODE_PADDING_LEFT = 10;
        private const float NODE_PADDING_RIGHT = 10;

        private const float DEFAULT_NODE_CONNECTION_OUT_SIZE_X = 20;
        private const float DEFAULT_NODE_CONNECTION_OUT_SIZE_Y = 20;

        private const float DEFAULT_NODE_CONNECTION_OUT_POSITION_X = DEFAULT_NODE_WINDOW_SIZE_X - DEFAULT_NODE_CONNECTION_OUT_SIZE_X;
        private const float DEFAULT_NODE_CONNECTION_OUT_POSITION_Y = DEFAULT_NODE_WINDOW_SIZE_Y * 0.5f - DEFAULT_NODE_CONNECTION_OUT_SIZE_Y * 0.5f;

        private const float DEFAULT_NODE_CONNECTION_OUT_MARGIN_LEFT = 10;

        private readonly Rect defaultNodeConnectionOutRect = new Rect(DEFAULT_NODE_CONNECTION_OUT_POSITION_X, DEFAULT_NODE_CONNECTION_OUT_POSITION_Y, DEFAULT_NODE_CONNECTION_OUT_SIZE_X, DEFAULT_NODE_CONNECTION_OUT_SIZE_Y);

        private const float DEFAULT_NODE_CONNECTION_IN_SIZE_X = 20;
        private const float DEFAULT_NODE_CONNECTION_IN_SIZE_Y = 20;

        private const float DEFAULT_NODE_CONNECTION_IN_POSITION_X = 0;
        private const float DEFAULT_NODE_CONNECTION_IN_POSITION_Y = DEFAULT_NODE_WINDOW_SIZE_Y * 0.5f - DEFAULT_NODE_CONNECTION_OUT_SIZE_Y * 0.5f;

        private const float DEFAULT_NODE_CONNECTION_IN_MARGIN_RIGHT = 10;

        private readonly Rect defaultNodeConnectionInRect = new Rect(DEFAULT_NODE_CONNECTION_IN_POSITION_X, DEFAULT_NODE_CONNECTION_IN_POSITION_Y, DEFAULT_NODE_CONNECTION_IN_SIZE_X, DEFAULT_NODE_CONNECTION_IN_SIZE_Y);

        private const float DEFAULT_SPEAKER_NAME_TEXT_SIZE_X = DEFAULT_NODE_WINDOW_SIZE_X - DEFAULT_NODE_CONNECTION_OUT_SIZE_X - DEFAULT_NODE_CONNECTION_OUT_MARGIN_LEFT - DEFAULT_NODE_CONNECTION_IN_SIZE_X - DEFAULT_NODE_CONNECTION_IN_MARGIN_RIGHT;
        private const float DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y = 20;

        private const float DEFAULT_SPEAKER_NAME_TEXT_POSITION_X = DEFAULT_NODE_CONNECTION_IN_POSITION_X + DEFAULT_NODE_CONNECTION_IN_SIZE_X + DEFAULT_NODE_CONNECTION_IN_MARGIN_RIGHT;
        private const float DEFAULT_SPEAKER_NAME_TEXT_POSITION_Y = NODE_PADDING_TOP;

        private const float DEFAULT_SPEAKER_NAME_TEXT_MARGIN_BOTTOM = 5;

        private readonly Rect speakerNameTextRect = new Rect(DEFAULT_SPEAKER_NAME_TEXT_POSITION_X, DEFAULT_SPEAKER_NAME_TEXT_POSITION_Y, DEFAULT_SPEAKER_NAME_TEXT_SIZE_X, DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y);

        private const float DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_X = DEFAULT_SPEAKER_NAME_TEXT_SIZE_X;
        private const float DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_Y = DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y;

        private const float DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_X = DEFAULT_SPEAKER_NAME_TEXT_POSITION_X;
        private const float DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_Y = NODE_PADDING_TOP + DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y + DEFAULT_SPEAKER_NAME_TEXT_MARGIN_BOTTOM;

        private const float DEFAULT_SPEAKER_NAME_INPUT_FIELD_MARGIN_BOTTOM = 10;

        private readonly Rect speakerNameInputFieldRect = new Rect(DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_X, DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_Y, DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_X, DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y);

        #region Normal Node Design

        private const float NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_X = DEFAULT_SPEAKER_NAME_TEXT_SIZE_X;
        private const float NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_Y = DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y;

        private const float NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_X = DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_X;
        private const float NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_Y = DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_Y + DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_Y + DEFAULT_SPEAKER_NAME_INPUT_FIELD_MARGIN_BOTTOM;

        private const float NORMAL_NODE_INPUT_FIELD_TEXT_MARGIN_BOTTOM = 5;

        private readonly Rect normalNodeInputText = new Rect(NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_X, NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_Y, NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_X, NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_Y);

        private const float NORMAL_NODE_INPUT_FIELD_SIZE_X = NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_X;
        private const float NORMAL_NODE_INPUT_FIELD_SIZE_Y = DEFAULT_NODE_WINDOW_SIZE_Y - NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_Y - NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_Y - NORMAL_NODE_INPUT_FIELD_TEXT_MARGIN_BOTTOM - NODE_PADDING_BOTTOM;

        private const float NORMAL_NODE_INPUT_FIELD_POSITION_X = NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_X;
        private const float NORMAL_NODE_INPUT_FIELD_POSITION_Y = NORMAL_NODE_INPUT_FIELD_TEXT_POSITION_Y + NORMAL_NODE_INPUT_FIELD_TEXT_SIZE_Y + NORMAL_NODE_INPUT_FIELD_TEXT_MARGIN_BOTTOM;

        private readonly Rect normalNodeInputFieldRect = new Rect(NORMAL_NODE_INPUT_FIELD_POSITION_X, NORMAL_NODE_INPUT_FIELD_POSITION_Y, NORMAL_NODE_INPUT_FIELD_SIZE_X, NORMAL_NODE_INPUT_FIELD_SIZE_Y);
        #endregion

        #region Choice Node Design

        private const float DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_X = DEFAULT_SPEAKER_NAME_TEXT_SIZE_X;
        private const float DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_Y = DEFAULT_SPEAKER_NAME_TEXT_SIZE_Y;

        private const float DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_X = DEFAULT_SPEAKER_NAME_TEXT_POSITION_X;
        private const float DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_Y = DEFAULT_SPEAKER_NAME_INPUT_FIELD_POSITION_Y + DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_Y + DEFAULT_SPEAKER_NAME_INPUT_FIELD_MARGIN_BOTTOM;

        private const float DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_MARGIN_BOTTOM = 5;

        private readonly Rect choiceAddOrDeleteTextRect = new Rect(DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_X, DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_Y, DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_X, DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_Y);

        private const float DEFAULT_CHOICE_ADD_BUTTON_SIZE_X = 40;
        private const float DEFAULT_CHOICE_ADD_BUTTON_SIZE_Y = 20;

        private const float DEFAULT_CHOICE_ADD_BUTTON_POSITION_X = DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_X;
        private const float DEFAULT_CHOICE_ADD_BUTTON_POSITION_Y = DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_Y + DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_Y + DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_MARGIN_BOTTOM;

        private readonly Rect choiceAddButtonRect = new Rect(DEFAULT_CHOICE_ADD_BUTTON_POSITION_X, DEFAULT_CHOICE_ADD_BUTTON_POSITION_Y, DEFAULT_CHOICE_ADD_BUTTON_SIZE_X, DEFAULT_CHOICE_ADD_BUTTON_SIZE_Y);

        private const float DEFAULT_CHOICE_DELETE_BUTTON_SIZE_X = 40;
        private const float DEFAULT_CHOICE_DELETE_BUTTON_SIZE_Y = 20;

        private const float DEFAULT_CHOICE_DELETE_BUTTON_POSITION_X = DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_POSITION_X + DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_SIZE_X - DEFAULT_CHOICE_DELETE_BUTTON_SIZE_X;
        private const float DEFAULT_CHOICE_DELETE_BUTTON_POSITION_Y = DEFAULT_CHOICE_ADD_BUTTON_POSITION_Y;

        private const float DEFAULT_CHOICE_DELETE_BUTTON_MARGIN_BOTTOM = 10;

        private readonly Rect choiceDeleteButtonRect = new Rect(DEFAULT_CHOICE_DELETE_BUTTON_POSITION_X, DEFAULT_CHOICE_DELETE_BUTTON_POSITION_Y, DEFAULT_CHOICE_DELETE_BUTTON_SIZE_X, DEFAULT_CHOICE_DELETE_BUTTON_SIZE_Y);

        private const float CHOICE_CONVERSATION_INPUT_FIELD_SIZE_X = DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_X;
        private const float CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y = DEFAULT_SPEAKER_NAME_INPUT_FIELD_SIZE_Y;

        private const float FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_X = DEFAULT_CHOICE_ADD_BUTTON_POSITION_X;
        private const float FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_Y = DEFAULT_CHOICE_DELETE_BUTTON_POSITION_Y + DEFAULT_CHOICE_DELETE_BUTTON_SIZE_Y + DEFAULT_CHOICE_DELETE_BUTTON_MARGIN_BOTTOM;

        private const float CHOICE_CONVERSATION_INPUT_FIELD_MARGIN_BOTTOM = 5;

        private const float CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_X = DEFAULT_NODE_CONNECTION_OUT_SIZE_X;
        private const float CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_Y = DEFAULT_NODE_CONNECTION_OUT_SIZE_Y;

        private const float FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_X = DEFAULT_NODE_WINDOW_SIZE_X - CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_X;
        private const float FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_Y = FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_Y;

        #endregion

        #region Start Conversation Node Design

        private const float START_CONVERSATION_NODE_WINDOW_SIZE_X = 80;
        private const float START_CONVERSATION_NODE_WINDOW_SIZE_Y = 70;

        private readonly Vector2 startNodeWindowSize = new Vector2(START_CONVERSATION_NODE_WINDOW_SIZE_X, START_CONVERSATION_NODE_WINDOW_SIZE_Y);

        private const float START_NODE_CONNECTION_OUT_SIZE_X = DEFAULT_NODE_CONNECTION_OUT_SIZE_X;
        private const float START_NODE_CONNECTION_OUT_SIZE_Y = DEFAULT_NODE_CONNECTION_OUT_SIZE_Y;

        private const float START_NODE_CONNECTION_OUT_POSITION_X = START_CONVERSATION_NODE_WINDOW_SIZE_X - START_NODE_CONNECTION_OUT_SIZE_X;
        private const float START_NODE_CONNECTION_OUT_POSITION_Y = START_CONVERSATION_NODE_WINDOW_SIZE_Y * 0.5f - START_NODE_CONNECTION_OUT_SIZE_Y * 0.5f;

        private readonly Rect startNodeConnectionOutRect = new Rect(START_NODE_CONNECTION_OUT_POSITION_X, START_NODE_CONNECTION_OUT_POSITION_Y, START_NODE_CONNECTION_OUT_SIZE_X, START_NODE_CONNECTION_OUT_SIZE_Y);

        #endregion

        #region End Conversation Node Design

        private const float END_CONVERSATION_NODE_WINDOW_SIZE_X = 80;
        private const float END_CONVERSATION_NODE_WINDOW_SIZE_Y = 70;

        private readonly Vector2 endNodeWindowSize = new Vector2(END_CONVERSATION_NODE_WINDOW_SIZE_X, END_CONVERSATION_NODE_WINDOW_SIZE_Y);

        private const float END_NODE_CONNECTION_IN_SIZE_X = DEFAULT_NODE_CONNECTION_IN_SIZE_X;
        private const float END_NODE_CONNECTION_IN_SIZE_Y = DEFAULT_NODE_CONNECTION_IN_SIZE_Y;

        private const float END_NODE_CONNECTION_IN_POSITION_X = 0;
        private const float END_NODE_CONNECTION_IN_POSITION_Y = START_CONVERSATION_NODE_WINDOW_SIZE_Y * 0.5f - START_NODE_CONNECTION_OUT_SIZE_Y * 0.5f;

        private readonly Rect endNodeConnectionInRect = new Rect(END_NODE_CONNECTION_IN_POSITION_X, END_NODE_CONNECTION_IN_POSITION_Y, END_NODE_CONNECTION_IN_SIZE_X, END_NODE_CONNECTION_IN_SIZE_Y);


        #endregion

        #endregion

        #region Connection Line Management

        private bool isDrawingLine = false;

        /// <summary>
        /// args : int startNodeId, ConnectionLineInfo lineInfo
        /// </summary>
        private Dictionary<int, ConnectionLineInfo> connectionLineInfoTable = new();

        private Vector2 drawStartLocalPosition = Vector2.zero;
        private int drawStartNodeId = -1;

        #endregion

        #region Start, End Node Settings

        private bool isStartNodeCreated = false;

        #endregion


        [MenuItem("Window/ConversationMakerWindow")]
        public static void ShowWindow()
        {
            // MyWindow 타입의 첫 번째 EditorWindow를 반환하거나, 없다면 새 창을 생성하고 보여줍니다.
            ConversationMakerWindow window = GetWindow<ConversationMakerWindow>("Conversation Maker !");
            window.Show();
        }

        private void OnEnable()
        {
            Debug.Log("ConversationMakerWindow OnEnable");

            backgroundMenu = new GenericMenu();

            backgroundMenu.AddItem(new GUIContent("Create Conversation Node"), false, ReadyToCreateNormalConversationNode);
            backgroundMenu.AddItem(new GUIContent("Create Choice Conversation Node"), false, ReadyToCreateChoiceConversationNode);
            backgroundMenu.AddItem(new GUIContent("Create Start Conversation Node"), false, ReadyToCreateStartConversationNode);
            backgroundMenu.AddItem(new GUIContent("Create End Conversation Node"), false, ReadyToCreateEndConversationNode);
        }

        private void OnDisable()
        {
            Debug.Log("ConversationMakerWindow OnDisable");
        }

        void OnGUI()
        {
            GUI.Label(conversationNameRect, "Conversation Name :");
            conversationNameInput = GUI.TextField(conversationNameInputFieldRect, conversationNameInput);

            backgroundRect = new Rect(10, 40, position.width - 20, position.height - 50);
            GUI.Box(backgroundRect, "");


            //GUI.Button()
            //GUI.Box(new Rect(0, 0, position.width / 2, position.height), "Left Area");


            //GUI.Button(new Rect(10, 80, 40, 20), "+");
            //id = EditorGUILayout.IntField("Instance ID:", id);

            //if (GUILayout.Button("Find Name"))
            //{
            //    Object obj = EditorUtility.InstanceIDToObject(id);

            //    if (!obj)
            //        Debug.LogError("No object could be found with instance id: " + id);
            //    else
            //        Debug.Log("Object's name: " + obj.name);
            //}

            // 마우스 오른쪽 버튼을 누르면 컨텍스트 메뉴를 표시합니다.
            if (Event.current.type == EventType.ContextClick && isDrawingLine == false)
            {
                backgroundMenu.ShowAsContext();
                currentMouseEvent = Event.current;
            }

            DrawNodes();
            DrawConnectionLines();

            //Handles.DrawBezier()

            //if (GUILayout.Button("Open Asset"))
            //{
            //    // "Assets/Example.png"라는 이미지 자산을 엽니다.
            //    Object obj = AssetDatabase.LoadAssetAtPath("Assets/CommonRPG/Scripts/UnitManager.cs", typeof(Object));
            //    AssetDatabase.OpenAsset(obj);
            //}

            if (isDrawingLine)
            {
                Handles.BeginGUI();

                NodeWindow drawStartNodeWindow = nodeWindows[drawStartNodeId];

                Vector2 drawStartPosition = new Vector2(drawStartNodeWindow.NodeRect.x + drawStartLocalPosition.x, drawStartNodeWindow.NodeRect.y + drawStartLocalPosition.y);
                Handles.DrawAAPolyLine(5, drawStartPosition, Event.current.mousePosition);

                Handles.EndGUI();
            }
        }

        private void MenuCallback(object test)
        {

        }

        [OnOpenAsset(1)]
        public static bool step1(int instanceID, int line, int col)
        {
            string name = EditorUtility.InstanceIDToObject(instanceID).name;
            Debug.Log("Open Asset step: 1 (" + name + ")");
            Debug.Log($"line ? :{line}  colum ? :{col}");
            return false; // we did not handle the open
        }

        [OnOpenAsset(2)]
        public static bool step2(int instanceID, int line)
        {
            Debug.Log("Open Asset step: 2 (" + instanceID + ")");
            return false; // we did not handle the open
        }

        private void ReadyToCreateNormalConversationNode()
        {
            Vector2 currentMousePosition = currentMouseEvent.mousePosition;

            NodeWindow newNode = new NodeWindow();

            newNode.NodeRect = new Rect(currentMousePosition.x, currentMousePosition.y, defaultNodeWindowSize.x, defaultNodeWindowSize.y);
            newNode.NodeName = "Normal Node";
            newNode.NodeType = ENodeType.Normal;
            newNode.NodeId = nodeWindows.Count;

            newNode.Conversations.Add("");

            nodeWindows.Add(newNode);
        }

        private void ReadyToCreateChoiceConversationNode()
        {
            Vector2 currentMousePosition = currentMouseEvent.mousePosition;

            NodeWindow newNode = new NodeWindow();

            newNode.NodeRect = new Rect(currentMousePosition.x, currentMousePosition.y, defaultNodeWindowSize.x, defaultNodeWindowSize.y);
            newNode.NodeName = "Choice Node";
            newNode.NodeType = ENodeType.Choice;
            newNode.NodeId = nodeWindows.Count;

            nodeWindows.Add(newNode);
        }

        private void ReadyToCreateStartConversationNode()
        {
            if (isStartNodeCreated)
            {
                Debug.LogError("Start Node must exist only 1");
                return;
            }

            Vector2 currentMousePosition = currentMouseEvent.mousePosition;

            NodeWindow newNode = new NodeWindow();

            newNode.NodeRect = new Rect(currentMousePosition.x, currentMousePosition.y, startNodeWindowSize.x, startNodeWindowSize.y);
            newNode.NodeName = "Start Node";
            newNode.NodeType = ENodeType.Start;
            newNode.NodeId = nodeWindows.Count;

            nodeWindows.Add(newNode);
            isStartNodeCreated = true;
        }

        private void ReadyToCreateEndConversationNode()
        {
            Vector2 currentMousePosition = currentMouseEvent.mousePosition;

            NodeWindow newNode = new NodeWindow();

            newNode.NodeRect = new Rect(currentMousePosition.x, currentMousePosition.y, endNodeWindowSize.x, endNodeWindowSize.y);
            newNode.NodeName = "End Node";
            newNode.NodeType = ENodeType.End;
            newNode.NodeId = nodeWindows.Count;

            nodeWindows.Add(newNode);
        }


        private void DrawNodes()
        {
            if (nodeWindows.Count == 0)
            {
                return;
            }

            BeginWindows();

            int nodeWindowsCount = nodeWindows.Count;

            for (int i = 0; i < nodeWindowsCount; ++i)
            {
                NodeWindow nodeWindow = nodeWindows[i];

                if (nodeWindow.NodeType == ENodeType.Choice)
                {
                    float inputFieldSizeY = CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y + CHOICE_CONVERSATION_INPUT_FIELD_MARGIN_BOTTOM;
                    float neededHeight = DEFAULT_CHOICE_ADD_BUTTON_POSITION_Y + DEFAULT_CHOICE_ADD_BUTTON_SIZE_Y + DEFAULT_CHOICE_ADD_OR_DELETE_TEXT_MARGIN_BOTTOM + NODE_PADDING_BOTTOM + inputFieldSizeY * nodeWindows[i].Conversations.Count;

                    if (defaultNodeWindowSize.y - 20 < neededHeight)
                    {
                        nodeWindow.NodeRect.height = neededHeight;
                    }
                }

                nodeWindow.NodeRect = GUI.Window(i, nodeWindow.NodeRect, OnDrawNodeWindowDetails, nodeWindow.NodeName);
            }

            EndWindows();
        }

        private void OnDrawNodeWindowDetails(int id)
        {
            NodeWindow nodeWindow = nodeWindows[id];
            ENodeType nodeType = nodeWindow.NodeType;

            switch(nodeType)
            {
                case ENodeType.Normal:
                {
                    DrawNornalNodeWindowDetails(nodeWindow);
                    break;
                }
                case ENodeType.Choice:
                {
                    DrawChiceNodeWindowDetails(nodeWindow);
                    break;
                }
                case ENodeType.Start:
                {
                    DrawStartNodeWindowDetails(nodeWindow);
                    break;
                }
                case ENodeType.End:
                {
                    DrawEndNodeWindowDetails(nodeWindow);
                    break;
                }
                default:
                {
                    Debug.LogError("Weird Node Detected....");
                    return;
                }
            }
            
            
            
            

            //List<string> conversations = nodeWindow.Conversations;

            //GUI.Label(speakerNameTextRect, "Speaker Name");
            //nodeWindow.SpeakerName = GUI.TextField(speakerNameInputFieldRect, nodeWindow.SpeakerName);

            //if (nodeWindow.NodeType != ENodeType.Start)
            //{
            //    Rect nodeConnectionInRect;

            //    if (nodeWindow.NodeType == ENodeType.End)
            //    {
            //        nodeConnectionInRect = endNodeConnectionInRect;
            //    }
            //    else
            //    {
            //        nodeConnectionInRect = defaultNodeConnectionInRect;
            //    }

            //    if (GUI.Button(nodeConnectionInRect, "I"))
            //    {
            //        if (Event.current.button == 0 && isDrawingLine)
            //        {
            //            isDrawingLine = false;

            //            if (id == drawStartNodeId)
            //            {
            //                return;
            //            }

            //            Vector2 drawEndLocalPosition = new Vector2(nodeConnectionInRect.x, nodeConnectionInRect.y + nodeConnectionInRect.height * 0.5f);
            //            int drawEndNodeId = id;

            //            if (nodeWindows[drawStartNodeId].NodeType == ENodeType.Normal || nodeWindows[drawStartNodeId].NodeType == ENodeType.End)
            //            {
            //                if (connectionLineInfoTable.ContainsKey(drawStartNodeId))
            //                {
            //                    Debug.LogError("Line Add Failed. Node Connection Out can only connect to 1 Node Connection In");
            //                    return;
            //                }
            //                else
            //                {
            //                    ConnectionLineInfo connectionLineInfo = new ConnectionLineInfo();

            //                    connectionLineInfo.startNodeConnectionlocalPositions.Add(drawStartLocalPosition);
            //                    connectionLineInfo.endNodeConnectionlocalPositions.Add(drawEndLocalPosition);

            //                    connectionLineInfo.startNodeWindow = nodeWindows[drawStartNodeId];
            //                    connectionLineInfo.endNodeWindows.Add(nodeWindows[drawEndNodeId]);

            //                    connectionLineInfoTable.Add(drawStartNodeId, connectionLineInfo);
            //                }
            //            }
            //            else
            //            {
            //                ConnectionLineInfo connectionLineInfo;

            //                if (connectionLineInfoTable.ContainsKey(drawStartNodeId))
            //                {
            //                    connectionLineInfo = connectionLineInfoTable[drawStartNodeId];
            //                }
            //                else
            //                {
            //                    connectionLineInfo = new ConnectionLineInfo();
            //                }

            //                connectionLineInfo.startNodeConnectionlocalPositions.Add(drawStartLocalPosition);
            //                connectionLineInfo.endNodeConnectionlocalPositions.Add(drawEndLocalPosition);

            //                connectionLineInfo.startNodeWindow = nodeWindows[drawStartNodeId];
            //                connectionLineInfo.endNodeWindows.Add(nodeWindows[drawEndNodeId]);

            //                if (connectionLineInfoTable.ContainsKey(drawEndNodeId) == false)
            //                {
            //                    connectionLineInfoTable.Add(drawStartNodeId, connectionLineInfo);
            //                }
            //            }
            //        }

            //        return;
            //    }
            //}

            //if (nodeWindow.NodeType == ENodeType.Normal)
            //{
            //    GUI.Label(normalNodeInputText, "Conversation Input");
            //    conversations[0] = GUI.TextArea(normalNodeInputFieldRect, conversations[0]);

            //    if (GUI.Button(defaultNodeConnectionOutRect, "O"))
            //    {
            //        //mouse left click is 0
            //        if (Event.current.button != 0)
            //        {
            //            return;
            //        }

            //        isDrawingLine = (isDrawingLine == false);

            //        if (isDrawingLine)
            //        {
            //            drawStartLocalPosition = new Vector2(defaultNodeConnectionOutRect.x + defaultNodeConnectionOutRect.width, defaultNodeConnectionOutRect.y + defaultNodeConnectionOutRect.height * 0.5f);
            //            drawStartNodeId = id;
            //        }
            //    }
            //}
            //else if (nodeWindow.NodeType == ENodeType.Choice)
            //{

            //    GUI.Label(choiceAddOrDeleteTextRect, "Add or Delete");

            //    if (GUI.Button(choiceAddButtonRect, "+"))
            //    {
            //        conversations.Add("");
            //    }

            //    // 윈도우 안에 인풋 필드를 삭제하는 버튼을 그림
            //    // 버튼을 누르면 리스트의 마지막 텍스트를 삭제함
            //    if (GUI.Button(choiceDeleteButtonRect, "-"))
            //    {
            //        if (conversations.Count > 0)
            //        {
            //            conversations.RemoveAt(conversations.Count - 1);
            //        }
            //    }

            //    // 리스트에 저장된 텍스트들을 인풋 필드로 그림
            //    // 인풋 필드의 텍스트가 변경되면 리스트에 반영됨
            //    int conversationsCount = conversations.Count;

            //    for (int i = 0; i < conversationsCount; ++i)
            //    {
            //        float margin = CHOICE_CONVERSATION_INPUT_FIELD_MARGIN_BOTTOM + CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y;

            //        Rect choiceConversationInputFieldRect = new Rect(FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_X, FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_Y + margin * i, CHOICE_CONVERSATION_INPUT_FIELD_SIZE_X, CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y);
            //        conversations[i] = GUI.TextArea(choiceConversationInputFieldRect, conversations[i]);

            //        Rect choiceConversationNodeConnectionOutRect = new Rect(FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_X, FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_Y + margin * i, CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_X, CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_Y);
            //        if (GUI.Button(choiceConversationNodeConnectionOutRect, "O"))
            //        {
            //            //mouse left click is 0
            //            if (Event.current.button != 0)
            //            {
            //                continue;
            //            }

            //            isDrawingLine = (isDrawingLine == false);

            //            if (isDrawingLine)
            //            {
            //                drawStartLocalPosition = new Vector2(choiceConversationNodeConnectionOutRect.x + choiceConversationNodeConnectionOutRect.width, choiceConversationNodeConnectionOutRect.y + choiceConversationNodeConnectionOutRect.height * 0.5f);
            //                drawStartNodeId = id;
            //            }

            //            break;
            //        }

            //    }
            //}

            //else if (nodeWindow.NodeType == ENodeType.Start)
            //{
            //    if (GUI.Button(startNodeConnectionOutRect, "O"))
            //    {
            //        //mouse left click is 0
            //        if (Event.current.button != 0)
            //        {
            //            return;
            //        }

            //        isDrawingLine = (isDrawingLine == false);

            //        if (isDrawingLine)
            //        {
            //            drawStartLocalPosition = new Vector2(defaultNodeConnectionOutRect.x + defaultNodeConnectionOutRect.width, defaultNodeConnectionOutRect.y + defaultNodeConnectionOutRect.height * 0.5f);
            //            drawStartNodeId = id;
            //        }
            //    }
            //}

            GUI.DragWindow();
        }

        private void DrawNornalNodeWindowDetails(NodeWindow nodeWindow)
        {
            List<string> conversations = nodeWindow.Conversations;
            int id = nodeWindow.NodeId;

            GUI.Label(speakerNameTextRect, "Speaker Name");
            nodeWindow.SpeakerName = GUI.TextField(speakerNameInputFieldRect, nodeWindow.SpeakerName);

            if (GUI.Button(defaultNodeConnectionInRect, "I"))
            {
                if (Event.current.button == 0 && isDrawingLine)
                {
                    isDrawingLine = false;

                    if (id == drawStartNodeId)
                    {
                        return;
                    }

                    Vector2 drawEndLocalPosition = new Vector2(defaultNodeConnectionInRect.x, defaultNodeConnectionInRect.y + defaultNodeConnectionInRect.height * 0.5f);
                    int drawEndNodeId = id;

                    if (connectionLineInfoTable.ContainsKey(drawStartNodeId))
                    {
                        Debug.LogError("Line Add Failed. Node Connection In can only connect to 1 Node Connection Out");
                        return;
                    }
                    else
                    {
                        ConnectionLineInfo connectionLineInfo = new ConnectionLineInfo();

                        connectionLineInfo.startNodeConnectionlocalPositions.Add(drawStartLocalPosition);
                        connectionLineInfo.endNodeConnectionlocalPositions.Add(drawEndLocalPosition);

                        connectionLineInfo.startNodeWindow = nodeWindows[drawStartNodeId];
                        connectionLineInfo.endNodeWindows.Add(nodeWindows[drawEndNodeId]);

                        connectionLineInfoTable.Add(drawStartNodeId, connectionLineInfo);
                    }
                }

                return;
            }

            GUI.Label(normalNodeInputText, "Conversation Input");
            conversations[0] = GUI.TextArea(normalNodeInputFieldRect, conversations[0]);

            if (GUI.Button(defaultNodeConnectionOutRect, "O"))
            {
                //mouse left click is 0
                if (Event.current.button != 0)
                {
                    return;
                }

                isDrawingLine = (isDrawingLine == false);

                if (isDrawingLine)
                {
                    drawStartLocalPosition = new Vector2(defaultNodeConnectionOutRect.x + defaultNodeConnectionOutRect.width, defaultNodeConnectionOutRect.y + defaultNodeConnectionOutRect.height * 0.5f);
                    drawStartNodeId = id;
                }
            }
        }

        private void DrawChiceNodeWindowDetails(NodeWindow nodeWindow)
        {
            List<string> conversations = nodeWindow.Conversations;
            int id = nodeWindow.NodeId;

            GUI.Label(speakerNameTextRect, "Speaker Name");
            nodeWindow.SpeakerName = GUI.TextField(speakerNameInputFieldRect, nodeWindow.SpeakerName);

            if (GUI.Button(defaultNodeConnectionInRect, "I"))
            {
                if (Event.current.button == 0 && isDrawingLine)
                {
                    isDrawingLine = false;

                    if (id == drawStartNodeId)
                    {
                        return;
                    }

                    Vector2 drawEndLocalPosition = new Vector2(defaultNodeConnectionInRect.x, defaultNodeConnectionInRect.y + defaultNodeConnectionInRect.height * 0.5f);
                    int drawEndNodeId = id;

                    ConnectionLineInfo connectionLineInfo;

                    if (connectionLineInfoTable.ContainsKey(drawStartNodeId))
                    {
                        connectionLineInfo = connectionLineInfoTable[drawStartNodeId];
                    }
                    else
                    {
                        connectionLineInfo = new ConnectionLineInfo();
                    }

                    connectionLineInfo.startNodeConnectionlocalPositions.Add(drawStartLocalPosition);
                    connectionLineInfo.endNodeConnectionlocalPositions.Add(drawEndLocalPosition);

                    connectionLineInfo.startNodeWindow = nodeWindows[drawStartNodeId];
                    connectionLineInfo.endNodeWindows.Add(nodeWindows[drawEndNodeId]);

                    if (connectionLineInfoTable.ContainsKey(drawEndNodeId) == false)
                    {
                        connectionLineInfoTable.Add(drawStartNodeId, connectionLineInfo);
                    }
                }

                return;
            }

            GUI.Label(choiceAddOrDeleteTextRect, "Add or Delete");

            if (GUI.Button(choiceAddButtonRect, "+"))
            {
                conversations.Add("");
            }

            // 윈도우 안에 인풋 필드를 삭제하는 버튼을 그림
            // 버튼을 누르면 리스트의 마지막 텍스트를 삭제함
            if (GUI.Button(choiceDeleteButtonRect, "-"))
            {
                if (conversations.Count > 0)
                {
                    conversations.RemoveAt(conversations.Count - 1);
                }
            }

            // 리스트에 저장된 텍스트들을 인풋 필드로 그림
            // 인풋 필드의 텍스트가 변경되면 리스트에 반영됨
            int conversationsCount = conversations.Count;

            for (int i = 0; i < conversationsCount; ++i)
            {
                float margin = CHOICE_CONVERSATION_INPUT_FIELD_MARGIN_BOTTOM + CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y;

                Rect choiceConversationInputFieldRect = new Rect(FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_X, FIRST_CHOICE_CONVERSATION_INPUT_FIELD_POSITION_Y + margin * i, CHOICE_CONVERSATION_INPUT_FIELD_SIZE_X, CHOICE_CONVERSATION_INPUT_FIELD_SIZE_Y);
                conversations[i] = GUI.TextArea(choiceConversationInputFieldRect, conversations[i]);

                Rect choiceConversationNodeConnectionOutRect = new Rect(FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_X, FIRST_CHOICE_CONVERSATION_NODE_CONNECTION_OUT_POSITION_Y + margin * i, CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_X, CHOICE_CONVERSATION_NODE_CONNECTION_OUT_SIZE_Y);
                if (GUI.Button(choiceConversationNodeConnectionOutRect, "O"))
                {
                    //mouse left click is 0
                    if (Event.current.button != 0)
                    {
                        continue;
                    }

                    isDrawingLine = (isDrawingLine == false);

                    if (isDrawingLine)
                    {
                        drawStartLocalPosition = new Vector2(choiceConversationNodeConnectionOutRect.x + choiceConversationNodeConnectionOutRect.width, choiceConversationNodeConnectionOutRect.y + choiceConversationNodeConnectionOutRect.height * 0.5f);
                        drawStartNodeId = id;
                    }

                    break;
                }
            }
        }

        private void DrawStartNodeWindowDetails(NodeWindow nodeWindow)
        {
            int id = nodeWindow.NodeId;

            if (GUI.Button(startNodeConnectionOutRect, "O") == false)
            {
                return;
            }

            //mouse left click is 0
            if (Event.current.button != 0)
            {
                return;
            }

            isDrawingLine = (isDrawingLine == false);

            if (isDrawingLine)
            {
                drawStartLocalPosition = new Vector2(startNodeConnectionOutRect.x + startNodeConnectionOutRect.width, startNodeConnectionOutRect.y + startNodeConnectionOutRect.height * 0.5f);
                drawStartNodeId = id;
            }
        }

        private void DrawEndNodeWindowDetails(NodeWindow nodeWindow)
        {
            List<string> conversations = nodeWindow.Conversations;
            int id = nodeWindow.NodeId;

            if (GUI.Button(endNodeConnectionInRect, "I") == false)
            {
                return;
            }

            if (Event.current.button == 0 && isDrawingLine)
            {
                isDrawingLine = false;

                if (id == drawStartNodeId)
                {
                    return;
                }

                Vector2 drawEndLocalPosition = new Vector2(endNodeConnectionInRect.x, endNodeConnectionInRect.y + endNodeConnectionInRect.height * 0.5f);
                int drawEndNodeId = id;

                if (connectionLineInfoTable.ContainsKey(drawStartNodeId))
                {
                    Debug.LogError("Line Add Failed. Node Connection In can only connect to 1 Node Connection Out");
                    return;
                }
                else
                {
                    ConnectionLineInfo connectionLineInfo = new ConnectionLineInfo();

                    connectionLineInfo.startNodeConnectionlocalPositions.Add(drawStartLocalPosition);
                    connectionLineInfo.endNodeConnectionlocalPositions.Add(drawEndLocalPosition);

                    connectionLineInfo.startNodeWindow = nodeWindows[drawStartNodeId];
                    connectionLineInfo.endNodeWindows.Add(nodeWindows[drawEndNodeId]);

                    connectionLineInfoTable.Add(drawStartNodeId, connectionLineInfo);
                }
            }
        }

        private void DrawConnectionLines()
        {
            if (connectionLineInfoTable.Count == 0)
            {
                return;
            }

            Handles.BeginGUI();

            foreach (var line in connectionLineInfoTable.Values)
            {
                int connectionsCount = line.startNodeConnectionlocalPositions.Count;

                for (int i = 0; i < connectionsCount; ++i)
                {
                    float startPositionX = line.startNodeWindow.NodeRect.x + line.startNodeConnectionlocalPositions[i].x;
                    float startPositionY = line.startNodeWindow.NodeRect.y + line.startNodeConnectionlocalPositions[i].y;
                    Vector2 startPosition = new Vector2(startPositionX, startPositionY);

                    float endPositionX = line.endNodeWindows[i].NodeRect.x + line.endNodeConnectionlocalPositions[i].x;
                    float endPositionY = line.endNodeWindows[i].NodeRect.y + line.endNodeConnectionlocalPositions[i].y;
                    Vector2 endPosition = new Vector2(endPositionX, endPositionY);

                    Handles.DrawAAPolyLine(5, startPosition, endPosition);
                }
            }

            Handles.EndGUI();
        }

        private class NodeWindow
        {
            public int NodeId = -1;
            public string NodeName;
            public ENodeType NodeType;
            public Rect NodeRect;

            public string SpeakerName;
            public List<string> Conversations = new();

        }

        private class ConnectionLineInfo
        {
            public NodeWindow startNodeWindow;
            //public NodeWindow endNodeWindow;
            public List<NodeWindow> endNodeWindows = new();

            //public Vector2 startNodeConnectionlocalPosition;
            //public Vector2 endNodeConnectionlocalPosition;

            public List<Vector2> startNodeConnectionlocalPositions = new();
            public List<Vector2> endNodeConnectionlocalPositions = new();
        }
    }


}
