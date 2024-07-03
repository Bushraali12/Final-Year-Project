using UnityEngine;
using UnityEngine.Video;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class VideoPlayerController : MonoBehaviour
{
    [System.Serializable]
    public class QuestionMaterial
    {
        public string questionNumber;
        public GameObject questionObj;
        public List<GameObject> answerObjs;
        public int correctAnswer;
    }

    public GameObject[] doors;
    public List<QuestionMaterial> questions;
    public GameObject Panel;
    public VideoPlayer[] videoClips; 
    public int videosWatchedCount = 0;
    public int videosToWatchForQuiz = 3; 
    private int currentQuestionIndex = 0;
    public GameObject correctObj;
    public GameObject wrongObj;
    public Button nextButton;
    public GameObject explorePanel;
  
    void Start()
    {
        if (doors != null && doors.Length > 0)
        {
            doors[0].SetActive(true);
        }
        Panel.SetActive(false);
        explorePanel.SetActive(false);
        StartCoroutine(WaitForVideos());
        // ShowQuiz();
    }

    IEnumerator WaitForVideos()
    {
        foreach (VideoPlayer videoPlayer in videoClips)
        {
            videoPlayer.loopPointReached += VideoFinished;
            yield return new WaitUntil(() => !videoPlayer.isPlaying);
            while (videoPlayer.isPlaying)
            {
                yield return null;
            }
        }
    }

    void VideoFinished(VideoPlayer vp)
    {
        vp.loopPointReached -= VideoFinished;
        videosWatchedCount++;
        if (videosWatchedCount == videosToWatchForQuiz)
        {
            ShowQuiz();
        }
    }

    void ShowQuiz()
    {
        Panel.SetActive(true);
        questions[currentQuestionIndex].questionObj.gameObject.SetActive(true);
        for (int i = 0; i < questions.Count; i++)
        {
            int questionIndex = i;
            foreach (var btn in questions[questionIndex].answerObjs)
                btn.GetComponent<Button>().onClick.AddListener(() => Check_Answer(questions[questionIndex].answerObjs.IndexOf(btn)));
        }
        nextButton.onClick.AddListener(() => NextQuestion()); 
    }

    private void NextQuestion()
    {
        currentQuestionIndex++;
        for (int i = 0; i < questions.Count; i++)
        {
            questions[i].questionObj.gameObject.SetActive(i == currentQuestionIndex);
        }
        correctObj.gameObject.SetActive(false);
        wrongObj.gameObject.SetActive(false);
        nextButton.gameObject.SetActive(false);
        if (currentQuestionIndex == questions.Count)
        {
            Panel.SetActive(false);
            Debug.Log("Panel is deactive");
            Debug.Log("Doors are openend");
            doors[0].SetActive(false);
            doors[1].SetActive(false);

            // Activate the explore panel
            explorePanel.SetActive(true);
            StartCoroutine(DisableExplorePanelAfterDelay(2f));
        }
    }

    private void Check_Answer(int answer)
    {
        if (questions[currentQuestionIndex].correctAnswer == answer)
        {
            correctObj.gameObject.SetActive(true);
            wrongObj.gameObject.SetActive(false);
            nextButton.gameObject.SetActive(true);
        }
        else
        {
            correctObj.gameObject.SetActive(false);
            wrongObj.gameObject.SetActive(true);
            nextButton.gameObject.SetActive(false);
        }
    }

    IEnumerator DisableExplorePanelAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);
        explorePanel.SetActive(false);
    }
}