using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sym4D;
using Sym = Sym4D.Sym4DEmulator;

public class Sym4DController : MonoBehaviour
{
    private int xPort;  // 의자와 통신할 포트 번호
    private int wPort;  // 팬 포트 번호

    private WaitForSeconds ws = new WaitForSeconds(0.1f);

    IEnumerator InitSym4DCoroutine()
    {
        // 포트 번호 추출
        xPort = Sym.Sym4D_X_Find();     // 의자 포트 할당
        yield return ws;

        wPort = Sym.Sym4D_W_Find();     // 팬 포트 할당
        yield return ws;

        // 의자 포트 오픈
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        // 의자의 회전각도 설정(roll, pitch) 의 max값
        Sym.Sym4D_X_SetConfig(100, 100);
        yield return ws;
        
        // 팬 포트 오픈
        Sym.Sym4D_W_StartContents(wPort);
        yield return ws;

        // 팬의 최대 풍량설정(Max)
        Sym.Sym4D_W_SetConfig(100);
        yield return ws;
    }

    IEnumerator SetMotionCoroutine(int roll, int pitch)
    {
        //  명령 전달 전 대기함수(포트 오픈)
        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        // 동작
        Sym.Sym4D_X_SendMosionData(roll * 10, pitch * 10);
        yield return ws;
    }

    IEnumerator SetWindCoroutine(int speed)
    {
        //  명령 전달 전 대기함수(포트 오픈)
        Sym.Sym4D_W_StartContents(wPort);
        yield return ws;

        // 동작
        Sym.Sym4D_W_SendMosionData(speed);
        yield return ws;
    }

    // 장비 테스트 동작
    IEnumerator TestDeviceCoroutine()
    {
        StartCoroutine(SetMotionCoroutine(-10, 0));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(10, 0));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(0, 0));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(0, -10));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(0, 10));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(-10, 10));
        yield return ws;

        StartCoroutine(SetMotionCoroutine(10, -10));
        yield return ws;

        StartCoroutine(SetWindCoroutine(100));
        yield return new WaitForSeconds(3.0f);

        StartCoroutine(SetWindCoroutine(0));
        yield return ws;

        Sym.Sym4D_X_EndContents();
    }

    // Start is called before the first frame update
    IEnumerator Start()
    {
        StartCoroutine(InitSym4DCoroutine());
        yield return ws;

        StartCoroutine(TestDeviceCoroutine());
        yield return ws;
    }

    // Update is called once per frame
    void Update()
    {
        currJoyX = Input.GetAxis("Horizontal");
        currJoyY = Input.GetAxis("Vertical");

        if(currJoyX != prevJoyX)
        {
            StartCoroutine(JoyStickMoveCoroutine());
            prevJoyX = currJoyX;
        }

        if(currJoyY != prevJoyY)
        {
            StartCoroutine(JoyStickMoveCoroutine());
            prevJoyY = currJoyY;
        }
    }

    float prevJoyX, prevJoyY;
    float currJoyX, currJoyY;
    IEnumerator JoyStickMoveCoroutine()
    {
        yield return ws;

        Sym.Sym4D_X_StartContents(xPort);
        yield return ws;

        Sym.Sym4D_X_SendMosionData((int) (-currJoyX * 100), (int)(currJoyY * 100));
        yield return ws;
    }
}