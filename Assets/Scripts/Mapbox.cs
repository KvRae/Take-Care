using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using System;

public class Mapbox : MonoBehaviour
{
    public string accessToken;
    public float zoom = 18.0f;
    public int bearing = 0;
    public int pitch = 0;
    public enum style { Light, Dark, Streets, Outdoors, Satellite, SatelliteStreets };
    public style mapStyle = style.Streets;
    public enum resolution { low = 1, high = 2 };
    public resolution mapResolution = resolution.low;
    private int mapWidth = 800;
    private int mapHeight = 600;
    private string[] styleStr = new string[] { "light-v18", "dark-v10", "streets-v11", "outdoors-v11", "satellite-v9", "satellite-streets-v11" };
    private string url = "";
    private bool mapIsLoading = false;
    private Rect rect;
    private bool updateMap = true;
    private string accessTokenLast;
    private float latitude;
    private float longitude;
    private float zoomLast = 18.0f;
    private int bearingLast = 0;
    private int pitchLast = 0;
    private style mapStyleLast = style.Streets;
    private resolution mapResolutionLast = resolution.low;

    public Button mapStyleButton;
    public Image markerImage;

    void Start()
    {
        StartCoroutine(UpdateMapRoutine());
        rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
        mapWidth = (int)Mathf.Round(rect.width);
        mapHeight = (int)Mathf.Round(rect.height);

        // Start GPS
        Input.location.Start();
        mapStyleButton.onClick.AddListener(ToggleMapStyle);

    }

    void Update()
    {
        // Check if the map needs to be updated
        if (updateMap && (accessTokenLast != accessToken || latitude != Input.location.lastData.latitude || longitude != Input.location.lastData.longitude || zoomLast != zoom ||
            bearingLast != bearing || pitchLast != pitch || mapStyleLast != mapStyle || mapResolutionLast != mapResolution))
        {
            rect = gameObject.GetComponent<RawImage>().rectTransform.rect;
            mapWidth = (int)Mathf.Round(rect.width);
            mapHeight = (int)Mathf.Round(rect.height);
            StartCoroutine(GetMapbox());
            updateMap = false;
        }
    }

    IEnumerator UpdateMapRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(5f);

            // Update latitude and longitude with GPS data
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;

            // Set updateMap to true to trigger the map update
            updateMap = true;
        }
    }

    IEnumerator GetMapbox()
    {
        url = "https://api.mapbox.com/styles/v1/mapbox/" + styleStr[(int)mapStyle] + "/static/" + longitude + "," + latitude + "," + zoom + "," + bearing + "," + pitch + "/" + mapWidth + "x" +
            mapHeight + "?" + "access_token=" + accessToken;
        mapIsLoading = true;
        UnityWebRequest www = UnityWebRequestTexture.GetTexture(url);
        yield return www.SendWebRequest();
        if (www.result != UnityWebRequest.Result.Success)
        {
            Debug.Log(" CWMW ERROR:" + www.error);
        }
        else
        {
            mapIsLoading = false;
            gameObject.GetComponent<RawImage>().texture = ((DownloadHandlerTexture)www.downloadHandler).texture;

            accessTokenLast = accessToken;
            latitude = Input.location.lastData.latitude;
            longitude = Input.location.lastData.longitude;
            zoomLast = zoom;
            bearingLast = bearing;
            pitchLast = pitch;
            mapStyleLast = mapStyle;
            mapResolutionLast = mapResolution;
            updateMap = true;
        }
    }

void UpdateMarkerPosition(float lat, float lon)
{
    // Convert latitude and longitude to Unity world coordinates
    Vector3 markerPosition = new Vector3(lon, 0, lat);

    // Set the UI Image's position
    RectTransform imageRectTransform = markerImage.rectTransform;
    imageRectTransform.anchoredPosition = markerPosition;
}

    void ToggleMapStyle()
      {
        mapStyle = (mapStyle == style.Streets) ? style.Satellite : style.Streets;
        updateMap = true; // Trigger an update when the map style changes
      }
}
