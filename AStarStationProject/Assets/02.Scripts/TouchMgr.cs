﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 사용자의 조작을 읽어 그 처리를 수행하는 클래스
public class TouchMgr : MonoBehaviour
{
    public GameObject startImg;
    public GameObject endImg;
    public bool canTouch = true;

    private RaycastHit2D hit;
    private int stationLayer;
    private string nowStation;
    private string destination;
    private AStar aStar;
    private Vector3 originCamPos;
    private Camera cam;

    void Start()
    {
        cam = Camera.main;
        stationLayer = LayerMask.NameToLayer("Station");
        aStar = FindObjectOfType<AStar>();
        startImg = Instantiate(startImg);
        startImg.SetActive(false);
        endImg = Instantiate(endImg);
        endImg.SetActive(false);
    }

    void Update()
    {
        if (!canTouch) return;

        if (Input.GetMouseButtonDown(0))
        {
            originCamPos = cam.transform.position;
        }

        if (Input.GetMouseButtonUp(0))
        {
            if (!originCamPos.Equals(cam.transform.position))
            {
                return;
            }
            hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (hit.collider != null)
            {
                TouchStation(hit.collider.name);
                return;
            }
            TouchOther();
        }
    }

    // 역을 클릭하였을 경우를 처리하는 함수
    private void TouchStation(string name)
    {
        if (!startImg.activeSelf)
        {
            startImg.SetActive(true);
            startImg.transform.position = GetModifiedPos(name);
            nowStation = name;
            return;
        }
        if (!endImg.activeSelf)
        {
            if (name.Equals(nowStation))
            {
                TouchOther();
                return;
            }
            endImg.SetActive(true);
            endImg.transform.position = GetModifiedPos(name);
            destination = name;
            canTouch = false;
            aStar.StartCoroutine(aStar.SearchPath(nowStation, destination));
            return;
        }
        TouchOther();
    }

    // 역 이외의 곳을 클릭하였을 경우를 처리하는 함수
    private void TouchOther()
    {
        if (!startImg.activeSelf)
        {
            return;
        }
        nowStation = null;
        startImg.SetActive(false);
        if (!endImg.activeSelf)
        {
            return;
        }
        aStar.DestroyRings();
        destination = null;
        endImg.SetActive(false);
    }

    // 화면 비율에 따른 화면제한범위를 반환하는 함수
    private Vector3 GetModifiedPos(string name)
    {
        Vector3 pos = GameObject.Find(name).transform.position;
        pos.x += 0.2f;
        pos.y += 0.4f;
        return pos;
    }
}
