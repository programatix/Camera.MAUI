﻿using Camera.MAUI.Plugin;
using Camera.MAUI.Plugin.ZXing;
using CommunityToolkit.Maui.Views;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Camera.MAUI.Test;

public class CameraViewModel : INotifyPropertyChanged
{
    private CameraInfo camera = null;
    private bool takeSnapshot = false;
    private bool autoStartPreview = false;
    private bool autoStartRecording = false;

    public CameraInfo Camera
    {
        get => camera;
        set
        {
            camera = value;
            OnPropertyChanged(nameof(Camera));
        }
    }

    private ObservableCollection<CameraInfo> cameras = new();

    public ObservableCollection<CameraInfo> Cameras
    {
        get => cameras;
        set
        {
            cameras = value;
            OnPropertyChanged(nameof(Cameras));
        }
    }

    public int NumCameras
    {
        set
        {
            if (value > 0)
                Camera = Cameras.First();
        }
    }

    private MicrophoneInfo micro = null;

    public MicrophoneInfo Microphone
    {
        get => micro;
        set
        {
            micro = value;
            OnPropertyChanged(nameof(Microphone));
        }
    }

    private ObservableCollection<MicrophoneInfo> micros = new();

    public ObservableCollection<MicrophoneInfo> Microphones
    {
        get => micros;
        set
        {
            micros = value;
            OnPropertyChanged(nameof(Microphones));
        }
    }

    public int NumMicrophones
    {
        set
        {
            if (value > 0)
                Microphone = Microphones.First();
        }
    }

    public bool AutoStartPreview
    {
        get => autoStartPreview;
        set
        {
            autoStartPreview = value;
            OnPropertyChanged(nameof(AutoStartPreview));
        }
    }

    public bool AutoStartRecording
    {
        get => autoStartRecording;
        set
        {
            autoStartRecording = value;
            OnPropertyChanged(nameof(AutoStartRecording));
        }
    }

    public MediaSource VideoSource { get; set; }
    public BarcodeDecoderOptions BarCodeOptions { get; set; }
    public string BarcodeText { get; set; } = "No barcode detected";
    private IPluginResult[] barCodeResults;

    public IPluginResult[] BarCodeResults
    {
        get => barCodeResults;
        set
        {
            barCodeResults = value;
            if (barCodeResults != null && barCodeResults.Length > 0)
            {
                if (barCodeResults is BarcodeResult[] results)
                {
                    BarcodeText = results[0].Text;
                }
            }
            else
                BarcodeText = "No barcode detected";
            OnPropertyChanged(nameof(BarcodeText));
        }
    }

    public bool TakeSnapshot
    {
        get => takeSnapshot;
        set
        {
            takeSnapshot = value;
            OnPropertyChanged(nameof(TakeSnapshot));
        }
    }

    public float SnapshotSeconds { get; set; } = 0f;

    public string Seconds
    {
        get => SnapshotSeconds.ToString();
        set
        {
            if (float.TryParse(value, out float seconds))
            {
                SnapshotSeconds = seconds;
                OnPropertyChanged(nameof(SnapshotSeconds));
            }
        }
    }

    public Command StartCamera { get; set; }
    public Command StopCamera { get; set; }
    public Command TakeSnapshotCmd { get; set; }
    public string RecordingFile { get; set; }
    public Command StartRecording { get; set; }
    public Command StopRecording { get; set; }

    public event PropertyChangedEventHandler PropertyChanged;

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public CameraViewModel()
    {
        BarCodeOptions = new ZXingDecoderOptions
        {
            AutoRotate = true,
            PossibleFormats = { Plugin.BarcodeFormat.QR_CODE },
            ReadMultipleCodes = false,
            TryHarder = true,
            TryInverted = true
        };
        OnPropertyChanged(nameof(BarCodeOptions));
        StartCamera = new Command(() =>
        {
            AutoStartPreview = true;
        });
        StopCamera = new Command(() =>
        {
            AutoStartPreview = false;
        });
        TakeSnapshotCmd = new Command(() =>
        {
            TakeSnapshot = false;
            TakeSnapshot = true;
        });
#if IOS
        RecordingFile = Path.Combine(FileSystem.Current.CacheDirectory, "Video.mov");
#else
        RecordingFile = Path.Combine(FileSystem.Current.CacheDirectory, "Video.mp4");
#endif
        OnPropertyChanged(nameof(RecordingFile));
        StartRecording = new Command(() =>
        {
            AutoStartRecording = true;
        });
        StopRecording = new Command(() =>
        {
            AutoStartRecording = false;
            VideoSource = MediaSource.FromFile(RecordingFile);
            OnPropertyChanged(nameof(VideoSource));
        });
        OnPropertyChanged(nameof(StartCamera));
        OnPropertyChanged(nameof(StopCamera));
        OnPropertyChanged(nameof(TakeSnapshotCmd));
        OnPropertyChanged(nameof(StartRecording));
        OnPropertyChanged(nameof(StopRecording));
    }
}