using System.Collections;
using UnityEngine;
using UnityEngine.Video;

public class BA_VideoManager : MonoBehaviour
{
    [SerializeField] RenderTexture targetTexture;
    public VideoPlayer videoPlayer;
    public Renderer videoRenderer;

    [SerializeField] private Texture originalTexture; // Store the original material for later use

    private void Start()
    {
        // Subscribe to the loopPointReached event
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached += OnVideoFinished;
        }
    }

    // Unsubscribe from events when the script is disabled or destroyed
    private void OnDisable()
    {
        if (videoPlayer != null)
        {
            videoPlayer.loopPointReached -= OnVideoFinished;
        }
    }

    // Event handler for the loopPointReached event
    private void OnVideoFinished(VideoPlayer vp)
    {
        // Revert the material texture to the original texture when the video finishes
            if (videoRenderer != null)
            {
                videoRenderer.material.mainTexture = originalTexture;
            }
    }

    public void OnPlay()
    {
        if (!videoPlayer.isPlaying)
        {
            // Revert the material texture to the original texture
            if (videoRenderer != null)
            {
                videoRenderer.material.mainTexture = originalTexture;
            }

            videoPlayer.Prepare();

            // Wait for video preparation to complete
            StartCoroutine(PrepareVideoAndPlay());
        }
    }

    private IEnumerator PrepareVideoAndPlay()
    {
        while (!videoPlayer.isPrepared)
        {
            yield return null;
        }

        // Change the material texture to the RenderTexture
        StartCoroutine(ChangeTextureWithDelay());
        videoPlayer.Play();
    }

    private IEnumerator ChangeTextureWithDelay()
    {
        // Add a short delay before changing the texture
        yield return new WaitForSeconds(0.5f); // Adjust the delay as needed

        // Change the material texture to the RenderTexture
        if (videoRenderer != null)
        {
            videoRenderer.material.mainTexture = targetTexture;
        }
    }

    public void OnStop()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Stop();

            // Revert the material texture to the original texture
            if (videoRenderer != null)
            {
                videoRenderer.material.mainTexture = originalTexture;
            }
        }
    }

    public void OnPause()
    {
        if (videoPlayer.isPlaying)
        {
            videoPlayer.Pause();

            // Revert the material texture to the original texture
            // if (videoRenderer != null)
            // {
            //     videoRenderer.material.mainTexture = originalTexture;
            // }
        }
    }
}

