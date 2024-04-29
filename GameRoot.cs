using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameRoot : MonoBehaviour
{
    public float step_timer = 0.0f;//����ð� ����
    private PlayerControl player = null;

    private AudioSource audio;
    public AudioClip jumpSound;

    public static string mes_text = "���� : ���콺 ���� ��ư\n ���ۿ� ������ ���� ����\n ������ ����� ���� �ӵ��� �������� ������ ũ�⵵ Ŀ����.";

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();

        this.audio = this.gameObject.AddComponent<AudioSource>();
        this.audio.clip = this.jumpSound;
        this.audio.loop = false;
    }

    void Update()
    {
        this.step_timer += Time.deltaTime;//����ð� ���ذ���

        if (this.player.isPlayEnd())
        {
            Application.LoadLevel("TitleScene");
        }

        this.audio.Play();
    }

    public float getPlayTime()//-->��ũ���������� create_floor_block()���� ���
    {
        float time;
        time = this.step_timer;
        return (time);//ȣ���� ���� ����ð��� �˷���
    }

    void OnGUI()
    {
        GUI.Label(new Rect(Screen.width / 2, 128, 1000, 1000), mes_text);
    }


}
