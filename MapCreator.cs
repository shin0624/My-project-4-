using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{ 
    public enum TYPE //��� ����
    {
        None = -1, //����
        FLOOR = 0,//�ٴ�
        HOLE,//����
        NUM,//��� ���� �� ����?(2)
    };

};


public class MapCreator : MonoBehaviour
    { private GameRoot game_root = null;

    public TextAsset Level_Data_text = null;

    public static float BLOCK_WIDTH = 1.0f;//��
    public static float BLOCK_HEIGHT = 0.2f;//����
    public static int BLOCK_NUM_IN_SCREEN = 24;//ȭ�� �� ���� ��� ����

    private LevelControl level_control = null; //���� ��Ʈ�Ѱ� ����Ǵ� ����

    private struct FloorBlock//��Ͽ� ���� ������ ��Ƽ� �����ϴ� ����ü(������ ������ ����)
    {
        public bool is_created;//��ϻ�������
        public Vector3 position;//�����ġ
    };

    private FloorBlock last_block;//�������� ������ ���
    private PlayerControl player = null;//scene ���� player�� ����
    private BlockCreator block_creator;//BlockCreator �� ����

    void Start()
    {
        this.player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerControl>();
        this.last_block.is_created = false;
        this.block_creator = this.gameObject.GetComponent<BlockCreator>();

        this.level_control = new LevelControl();
        this.level_control.initialize();

        this.level_control.loadLevelData(this.Level_Data_text);//LevelControl.cs�� �ִ� loadLevelData()�޼��带 ȣ��.

    this.game_root = this.gameObject.GetComponent<GameRoot>();//���ӷ�Ʈ ��ũ��Ʈ �Ҵ�
        this.player.level_control = this.level_control;
    }

    
    void Update()
    {
        float block_generate_x = this.player.transform.position.x;//�÷��̾��� x��ġ
        block_generate_x += BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN + 1) / 2.0f;//�뷫 �� ȭ�鸸ŭ ���������� �̵�, �� ��ġ�� ��� ���� ���ΰ�

        //�������� ���� ��� ��ġ�� ���ΰ� ������ ��-->����� ���鵵��
        while (this.last_block.position.x < block_generate_x)
        {
            this.create_floor_block();
        }
    }

    private void create_floor_block()
    {
        Vector3 block_position;//�������� ���� ����� ��ġ
        if(!this.last_block.is_created)//last_block�� �������� ���� ���
        {
            block_position = this.player.transform.position;//����� ��ġ�� player�� ����
            block_position.x -= BLOCK_WIDTH * ((float)BLOCK_NUM_IN_SCREEN / 2.0f);//����� x ��ġ�� ȭ�� ���ݸ�ŭ �������� �̵�
            block_position.y = 0.0f;//����� y ��ġ�� 0
        }
        else//last_block�� ������ ���
        {
            block_position = this.last_block.position;//�̹��� ���� ����� ��ġ�� ������ ���� ��ϰ� ����
        }
        block_position.x += BLOCK_WIDTH;//����� 1��ϸ�ŭ ���������� �̵�

    //BlockCreator ��ũ��Ʈ�� crateBlock()�� ������ ���� --> ���������� �ڵ忡�� ������ block_position�� �ǳ��ش�
    //this.block_creator.createBlock(block_position);

    //this.level_control.update();//������Ʈ�� ����
        this.level_control.update(this.game_root.getPlayTime());
        block_position.y = level_control.current_block.height * BLOCK_HEIGHT;//���� ���� ��� ������ ���̸� scene���� ��ǥ�� ��ȯ
        LevelControl.CreationInfo current = this.level_control.current_block;//���� ���� ��Ͽ� ���� ������ Ŀ��Ʈ ������ �ִ´�
        if(current.block_type == Block.TYPE.FLOOR)
        {
            this.block_creator.createBlock(block_position);//���� ���� ����� �ٴ��� ������ ��ġ�� ��� ����
        }

        this.last_block.position = block_position;//last_block�� ��ġ�� �̹� ��ġ�� ����
        this.last_block.is_created = true;//����� �����Ǿ����Ƿ� true�� ����
    }
    
}
