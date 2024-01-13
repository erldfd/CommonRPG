using UnityEditor;
using UnityEngine;
using System.Collections.Generic; // List를 사용하기 위해 추가

public class CustomWindow : EditorWindow
{
    [MenuItem("Window/My CustomWindow")]
    public static void ShowWindow()
    {
        // 창을 띄우는 함수
        GetWindow(typeof(CustomWindow));
    }

    // 윈도우의 위치와 크기를 저장할 변수
    private Rect window1 = new Rect(10, 10, 300, 200); // 윈도우의 크기를 넓힘
    private Rect window2 = new Rect(300, 10, 100, 100);

    // 입력 필드의 텍스트를 저장할 리스트
    private List<string> inputTexts = new List<string>();

    private void OnGUI()
    {
        // 창에 GUI 요소를 그리는 함수
        // BeginWindows와 EndWindows 사이에 윈도우를 그림
        BeginWindows();
        // 왼쪽 구역의 배경 색상을 빨간색으로 설정
        GUI.backgroundColor = Color.red;
        // 왼쪽 구역에 박스를 그림
        // 박스의 내용은 빈 문자열
        GUI.Box(new Rect(0, 0, position.width / 2, position.height), "");
        // 왼쪽 구역에 첫 번째 윈도우를 그림
        // 윈도우의 위치와 크기는 window1 변수에 저장된 값에 따름
        // 윈도우의 제목은 "Window 1"
        // 윈도우의 내용을 그리는 함수는 WindowFunction1
        window1 = GUI.Window(0, window1, WindowFunction1, "Window 1");
        // 오른쪽 구역의 배경 색상을 파란색으로 설정
        GUI.backgroundColor = Color.blue;
        // 오른쪽 구역에 박스를 그림
        // 박스의 내용은 빈 문자열
        GUI.Box(new Rect(position.width / 2, 0, position.width / 2, position.height), "");
        // 오른쪽 구역에 두 번째 윈도우를 그림
        // 윈도우의 위치와 크기는 window2 변수에 저장된 값에 따름
        // 윈도우의 제목은 "Window 2"
        // 윈도우의 내용을 그리는 함수는 WindowFunction2
        window2 = GUI.Window(1, window2, WindowFunction2, "Window 2");
        
        EndWindows();
    }

    // 첫 번째 윈도우의 내용을 그리는 함수
    private void WindowFunction1(int windowID)
    {
        
        // 윈도우 안에 텍스트를 그림
        GUI.Label(new Rect(10, 20, 80, 20), "Hello, world!");
        // 윈도우 안에 인풋 필드를 추가하는 버튼을 그림
        // 버튼을 누르면 리스트에 새로운 텍스트를 추가함
        if (GUI.Button(new Rect(10, 80, 40, 20), "+"))
        {
            inputTexts.Add("Enter text here");
        }
        // 윈도우 안에 인풋 필드를 삭제하는 버튼을 그림
        // 버튼을 누르면 리스트의 마지막 텍스트를 삭제함
        if (GUI.Button(new Rect(50, 80, 40, 20), "-"))
        {
            if (inputTexts.Count > 0)
            {
                inputTexts.RemoveAt(inputTexts.Count - 1);
            }
        }
        // 리스트에 저장된 텍스트들을 인풋 필드로 그림
        // 인풋 필드의 텍스트가 변경되면 리스트에 반영됨
        for (int i = 0; i < inputTexts.Count; i++)
        {
            inputTexts[i] = GUI.TextField(new Rect(10, 110 + i * 40, 80, 20), inputTexts[i]); // 인풋필드의 간격을 늘림
        }
        // 윈도우의 최소 크기와 최대 크기를 설정함
        minSize = new Vector2(100, 100);
        //maxSize = new Vector2(500, 500);
        // 윈도우의 우측 하단 모서리에 크기 조절 핸들을 그림
        // 핸들을 누르고 드래그하면 윈도우의 크기가 변경됨
        float mouseDeltaX = Event.current.delta.x;
        float mouseDeltaY = Event.current.delta.y;

        if (GUI.RepeatButton(new Rect(window1.width - 20, window1.height - 20, 20, 20), "↘"))
        {
            Debug.Log($"{Event.current.delta} : {mouseDeltaX}, {mouseDeltaY}");
            window1.width += 20;
            window1.height += 40;
        }
        // 윈도우를 드래그할 수 있도록 함
        GUI.DragWindow(new Rect(0, 0, position.width / 2, position.height));
    }

    // 두 번째 윈도우의 내용을 그리는 함수
    private void WindowFunction2(int windowID)
    {
        GUI.Button(new Rect(10, 80, 40, 20), "+");
        // 윈도우 안에 버튼을 그림
        // 버튼을 누르면 콘솔에 메시지를 출력함
        if (GUI.Button(new Rect(10, 20, 80, 20), "Click me"))
        {
            Debug.Log("Clicked!");
        }
        // 윈도우를 드래그할 수 있도록 함
        GUI.DragWindow();
    }
}
