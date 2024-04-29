using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    public static float ACCELERATION = 10.0f;//���ӵ�
    public static float SPEED_MIN = 4.0f;//�ӵ� �ּڰ�
    public static float SPEED_MAX = 8.0f;//�ӵ� �ִ�
    public static float JUMP_HEIGHT_MAX = 3.0f;//���� ����
    public static float JUMP_KEY_RELEASE_REDUCE = 0.5f;//���� ���� ���ӵ�

    public static float LOW_HEIGHT = -0.0f;//���ΰ����� ���� ��ҿ� �÷��̾ ���� ��, �� ���ۿ� ������ ��

    public float current_speed = 0.0f;//���� �ӵ�
    public LevelControl level_control = null;//LevelControl�� ����� --->LevelControl�� �����ϱ� ���� ���� 2�� �߰�

    private float click_timer = -1.0f;//��ư�� ���� ���� �ð�
    private float CLICK_GRACE_TIME = 0.5f;//�����ϰ���� �ǻ縦 �޾Ƶ��� �ð�

    //GUI�� ���� ���� �߰�
    private Text JumpScore;
    private int getCount = 0;

    public enum STEP//Player�� ���� ���¸� ��Ÿ���� �ڷ���
    {
        NONE = -1,//�������� ����
        RUN = 0,//�޸���
        JUMP,//����
        MISS,//����
        NUM,//���°� �� ���� �ִ��� �����ش�.(=3)

    };

    public STEP step = STEP.NONE;//�÷��̾� ���� ����
    public STEP next_step = STEP.NONE;//�÷��̾� ���� ����

    public float step_timer = 0.0f;//��� �ð�
    private bool is_landed = false;//���� ����
    private bool is_colided = false;//���𰡿� �浹�ߴ��� ����
    private bool is_key_released = false;//��ư�� ���������� ����

    void Start()
    {
        this.next_step = STEP.RUN;//������ �������ڸ��� �޸� �� �ֵ���

        JumpScore = GameObject.Find("Score").GetComponent<Text>();
    }

    private void check_landed()
    {
        this.is_landed = false;
        do
        {
            Vector3 s = this.transform.position;//�÷��̾� ������ġ
            Vector3 e = s + Vector3.down * 1.0f;//s���� �Ʒ��� 1.0f �̵��� ��ġ
            RaycastHit hit;
            if (!Physics.Linecast(s, e, out hit))//s���� e ���̿� �ƹ��͵� ���� ��
            {
                break;//�ƹ��͵� ���� �ʰ� do while ���� Ż��
            }
            if (this.step == STEP.JUMP) //s���� e ���̿� ���� ���� �� �Ʒ��� ó���� ����
            {
                if (this.step_timer < Time.deltaTime * 3.0f)//��� �ð��� 3.0f �̸��� ��
                {
                    break;
                }
            }
            this.is_landed = true;//s���� e ���̿� ���� �ְ� jump ���İ� �ƴ� �� ����
        }
        while (false);//���� Ż�ⱸ
    }

    void Update()
    {
        
        Vector3 velocity = this.GetComponent<Rigidbody>().velocity;//�ӵ� ����
        this.current_speed = this.level_control.getPlayerSpeed();
        this.check_landed();//���� �������� üũ

        switch (this.step) //���� ��ġ�� ���ΰ� �Ʒ��̸� ����
        {
            case STEP.RUN:
            case STEP.JUMP:
                if(this.transform.position.y < LOW_HEIGHT)
                {
                    this.next_step = STEP.MISS;
                }
                break;
        }

        this.step_timer += Time.deltaTime;//����ð� üũ

        if (Input.GetMouseButtonDown(0))
        {
            this.click_timer = 0.0f;//��ư�� �������� Ÿ�̸� ����
        }
        else
        {
            if(this.click_timer >= 0.0f)
            {
                this.click_timer += Time.deltaTime;//��ư�� �ȴ��ȴٸ� ����ð� ���ϱ�
            }
        }


        //���� ���°� ���������� ������ ������ ��ȭ�� ����
        if (this.next_step == STEP.NONE)
        {
            switch (this.step)//�÷��̾��� ������·� �б�
            {
                case STEP.RUN://�޸��� ���� ��
                    if(0.0f <=this.click_timer && this.click_timer <= CLICK_GRACE_TIME)//Ŭ�� Ÿ�̸Ӱ� 0 �̻�, Ŭ�� �׷��̽� Ÿ�� ������ ��
                    {
                        if (this.is_landed)
                        {//�����ߴٸ� ��ư�� ������ ���� ������ -1.0f ��������.
                            this.click_timer = -1.0f;
                            this.next_step = STEP.JUMP;
                        }
                    }
                    /*if (!this.is_landed)
                    {
                        //�޸�����, �������� ���� ��� �ൿ x
                    }
                    else
                    {
                        if (Input.GetMouseButtonDown(0))
                        {
                            this.next_step = STEP.JUMP;//�޸��� ��, ����, ���� ��ư ���� �� ���� ���¸� ������ ����
                        }
                    }*/
                    break;
                case STEP.JUMP://�������� ��
                    if (this.is_landed)
                    {
                        this.next_step = STEP.RUN;//���� �� ���� �� ���� ���¸� ��������.
                        getCount++;
                    }
                    break;
            }
        }
        while (this.next_step != STEP.NONE) // ���� ������ '�������� ����'�� �ƴ� ����. �� ���°� ���� ��
        {
            this.step = this.next_step; // ���� ���� = ���� ����
            this.next_step = STEP.NONE; // ���� ���� = ���� ����

            switch (this.step) // ���ŵ� ���� ���� ����ġ
            {
                case STEP.JUMP: // �ְ� ������ ���̱��� ������ �� �ִ� �ӵ� ���. sqrt�� �������� ���ϴ� �޼���.
                    velocity.y = Mathf.Sqrt(2.0f * 9.8f * PlayerControl.JUMP_HEIGHT_MAX); // ��ư�� ���������� ��Ÿ���� �÷��� Ŭ����
                    this.is_key_released = false;
                    break;
            }
            this.step_timer = 0.0f; // ���°� �������Ƿ� ����ð� ���� 
        }


        //���� �� �� ������ ���� ó��
        switch (this.step)
        {
            case STEP.RUN://�޸��� ���� ��
                velocity.x += PlayerControl.ACCELERATION * Time.deltaTime;

               /* if (Mathf.Abs(velocity.x) > PlayerControl.SPEED_MAX)//�ӵ��� �ְ�ӵ� ������ ���� ��
                {
                    velocity.x *= PlayerControl.SPEED_MAX / Mathf.Abs(this.GetComponent<Rigidbody>().velocity.x);
                }*/
               if(Mathf.Abs(velocity.x) > this.current_speed)//������� ���� �ӵ��� �����ؾ� �� �ӵ��� �Ѿ��ٸ� ���� �ʰ� ����
                {
                    velocity.x*=this.current_speed / Mathf.Abs(velocity.x);
                }
                break;
            case STEP.JUMP://�������� ��
                
                    if (!Input.GetMouseButtonUp(0))//��ư�� ������ ������ �ƴ� ��, �� ��ư�� ������ ���� ��.
                    {
                        break;
                    }
                    if (this.is_key_released)//�̹� ���ӵ� ������ ��(�� ���̻� �������� �ʵ���)
                    {
                        break;
                    }
                    if (velocity.y <= 0.0f)//���Ϲ��� �ӵ��� 0 ������ ��, �� �ϰ� ���� ��
                    {
                        break;
                    }
                    velocity.y *= JUMP_KEY_RELEASE_REDUCE;//��ư�� �������ְ� ��� ���� �� -->���� ����. ���� ��� ����
                
                

                this.is_key_released = true;
                break;

            case STEP.MISS:
                velocity.x -= PlayerControl.ACCELERATION * Time.deltaTime;//���ӵ��� ���� �÷��̾��� �ӵ��� ����
                if(velocity.x < 0.0f)//�÷��̾� �ӵ��� ���̳ʽ��� ��->0����
                {
                    velocity.x = 0.0f;
                }
                break;

                }
                 this.GetComponent<Rigidbody>().velocity = velocity;//Rigidbody�� �ӵ��� ������ ���� �ӵ��� ����

        SetCountText();
        
    }
       
    public bool isPlayEnd()//������ �������� ����
    {
        bool ret = false;
        switch(this.step)
        {
            case STEP.MISS:
                ret = true;
                break;
        }
        return (ret);
    }

    void SetCountText()
    {
        JumpScore.text = "������ Ƚ�� : " + getCount;
    }

    }
   

