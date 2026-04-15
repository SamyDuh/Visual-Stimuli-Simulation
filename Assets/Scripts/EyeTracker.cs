using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using VIVE.OpenXR;
using VIVE.OpenXR.EyeTracker;

public class Focus3PupilLogger : MonoBehaviour
{
    [Header("Logging")]
    public string fileName = "focus3_pupil_log.csv";
    public LightChange lightChange;
    public bool flushEveryNFrames = true;
    public int flushIntervalFrames = 300;

    private readonly List<string> rows = new();
    private string filePath;
    private int framesSinceFlush = 0;

    void Start()
    {
        filePath = Path.Combine(Application.persistentDataPath, fileName);

        rows.Add(
            "unity_time,frame,color," +
            "left_diameter_valid,left_diameter,left_position_valid,left_pos_x,left_pos_y," +
            "right_diameter_valid,right_diameter,right_position_valid,right_pos_x,right_pos_y"
        );

        Debug.Log("Pupil log path: " + filePath);
    }

    void Update()
    {
        XR_HTC_eye_tracker.Interop.GetEyePupilData(out XrSingleEyePupilDataHTC[] pupilData);

        XrSingleEyePupilDataHTC leftPupil =
            pupilData[(int)XrEyePositionHTC.XR_EYE_POSITION_LEFT_HTC];

        XrSingleEyePupilDataHTC rightPupil =
            pupilData[(int)XrEyePositionHTC.XR_EYE_POSITION_RIGHT_HTC];

        string currentColor = lightChange != null ? lightChange.GetCurrentColor() : "null";

        string row =
            F(Time.time) + "," +
            Time.frameCount + "," +
            "\"" + EscapeCsv(currentColor) + "\"," +

            Bool(leftPupil.isDiameterValid) + "," +
            F(leftPupil.isDiameterValid ? leftPupil.pupilDiameter : 0f) + "," +
            Bool(leftPupil.isPositionValid) + "," +
            F(leftPupil.isPositionValid ? leftPupil.pupilPosition.x : 0f) + "," +
            F(leftPupil.isPositionValid ? leftPupil.pupilPosition.y : 0f) + "," +

            Bool(rightPupil.isDiameterValid) + "," +
            F(rightPupil.isDiameterValid ? rightPupil.pupilDiameter : 0f) + "," +
            Bool(rightPupil.isPositionValid) + "," +
            F(rightPupil.isPositionValid ? rightPupil.pupilPosition.x : 0f) + "," +
            F(rightPupil.isPositionValid ? rightPupil.pupilPosition.y : 0f);

        rows.Add(row);

        if (flushEveryNFrames)
        {
            framesSinceFlush++;
            if (framesSinceFlush >= flushIntervalFrames)
            {
                FlushToDisk();
                framesSinceFlush = 0;
            }
        }
    }

    void OnApplicationQuit()
    {
        FlushToDisk();
        Debug.Log("Saved pupil log to: " + filePath);
    }

    void OnApplicationPause(bool pauseStatus)
    {
        if (pauseStatus)
            FlushToDisk();
    }

    private void FlushToDisk()
    {
        try
        {
            File.WriteAllLines(filePath, rows);
        }
        catch (System.Exception e)
        {
            Debug.LogError("Failed to write pupil log: " + e.Message);
        }
    }

    private static string F(float value)
    {
        return value.ToString("F6", CultureInfo.InvariantCulture);
    }

    private static string Bool(bool value)
    {
        return value ? "1" : "0";
    }

    private static string EscapeCsv(string value)
    {
        if (string.IsNullOrEmpty(value))
            return "";

        return value.Replace("\"", "\"\"");
    }
}