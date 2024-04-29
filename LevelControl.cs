using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LevelData
{
    public struct Range //����
    {
        public int min;
        public int max;
    };

    public float end_time;//����ð�
    public float player_speed;//�÷��̾� �ӵ�
    public Range floor_count;//���� ��� �� ����
    public Range hole_count;//���� ���� ����
    public Range height_diff;//���� ���� ����

    public LevelData()
    {
        this.end_time = 15.0f;
        this.player_speed = 6.0f;
        this.floor_count.min = 10;
        this.floor_count.max = 10;
        this.hole_count.min = 2;
        this.hole_count.max = 6;
        this.height_diff.min = 0;
        this.height_diff.max = 0;
            
    }
}

public class LevelControl : MonoBehaviour
{
    //�� ��ũ��Ʈ�� Ÿ�̹� �� ��� ���� ������ �����ϴ� ��ũ��Ʈ��, ���� ������Ʈ�� �������� ����. ���� �ڵ� ȣ�� �޼���� ��� X

    public float getPlayerSpeed()
    {
        return (this.level_datas[this.level].player_speed);
    }

    private List<LevelData> level_datas = new List<LevelData>();
    public int HEIGHT_MAX = 20;
    public int HEIGHT_MIN = -4;

    public struct CreationInfo //������ �ϴ� ��� ����. ����, ����, ����
    {
        public Block.TYPE block_type; //����� ����
        public int max_count; //��� �ִ� ����
        public int height; //��� ��ġ ����
        public int current_count; //�ۼ��� ����� ����
    };

    public CreationInfo previous_block; //������ � ����� ������°�?
    public CreationInfo current_block; // ���� � ����� ������ �ϴ°�?
    public CreationInfo next_block; // ������ � ����� ������ �ϴ°�?

    public int block_count = 0;//������ ��� �� ��
    public int level = 0;//���̵�
  
    private void clear_next_block(ref CreationInfo block) //������ ��Ʈ�� �ʱ갪�� �ִ´�. �ܺο��� ȣ��� ���� ���⿡ �����̺����� �ۼ�
    {
        block.block_type = Block.TYPE.FLOOR;
        block.max_count = 15;
        block.height = 0;
        block.current_count = 0;
    }

    public void initialize() //������ ��Ʈ �ʱ�ȭ
    {
        this.block_count = 0;
        this.clear_next_block(ref this.previous_block);//����
        this.clear_next_block(ref this.current_block);//����
        this.clear_next_block(ref this.next_block);//����
    }

    private void update_level(ref CreationInfo current, CreationInfo previous, float passage_time)//�� �μ� passage_time���� �÷��� ��� �ð��� �޵��� �߰�.
    {   //���� ���� ������ ���� ��ȯ��Ű���� �� �� Mathf.Repeat(value, max) ���
        float local_time = Mathf.Repeat(passage_time, this.level_datas[this.level_datas.Count - 1].end_time);//����1~5 �ݺ�.  ���� 5�� �� ��� �ٽ� ���� 1�� ����

        int i;
        for (i = 0; i < this.level_datas.Count - 1; i++)//���� ���� ���ϱ�
        {
            if(local_time <= this.level_datas[i].end_time)
            {
                break;
            }
        }
        this.level = i;

        current.block_type = Block.TYPE.FLOOR;
        current.max_count = 1;

        if(this.block_count >= 10)
        {
            //���� ������ �����͸� �����´�
            LevelData level_data;
            level_data = this.level_datas[this.level];

            switch (previous.block_type) 
            {   
                case Block.TYPE.FLOOR:
                    current.block_type = Block.TYPE.HOLE;//���� ����� �ٴ��� ��->���� ����
                    current.max_count = Random.Range(level_data.hole_count.min, level_data.hole_count.max);//���� ũ�� �ּڰ�~�ִ� ������ ������ ��
                    current.height = previous.height;break;
                case Block.TYPE.HOLE:
                    current.block_type = Block.TYPE.FLOOR;//���� ����� ������ ��->�ٴ� ����
                    current.max_count = Random.Range(level_data.floor_count.min, level_data.floor_count.max);
                    //�ٴ� ���� �ּڰ�~�ִ�
                    int height_min = previous.height + level_data.height_diff.min;
                    int height_max = previous.height + level_data.height_diff.max;
                    height_min = Mathf.Clamp(height_min, HEIGHT_MIN, HEIGHT_MAX);
                    height_max = Mathf.Clamp(height_max, HEIGHT_MIN, HEIGHT_MAX);//Mathf.Clamp(value, min, max) = ���� �ּڰ�~�ִ� ���� ���� ������ �ֱ� ���� ���

                    current.height =Random.Range(height_min, height_max);break;
            }

        }
    }

    
    public void update(float passage_time)//�ݺ� ������ �ʿ��� ���� ó��
    {
        this.current_block.current_count++;//�̹��� ���� ��� ���� ����
        if(this.current_block.current_count >= this.current_block.max_count)
        {
            this.previous_block = this.current_block;//�̹��� ���� ��� ���� >= �ƽ� ī��Ʈ
            this.current_block = this.next_block;      
            this.clear_next_block(ref this.next_block);//������ ���� ��� ���� �ʱ�ȭ
            this.update_level(ref this.next_block, this.current_block, passage_time);//������ ���� ��� ����
        }
        this.block_count++;//��� �� �� ����
    }

    public void loadLevelData(TextAsset Level_Data_text)//�ؽ�Ʈ���� �ε� �޼���
    {
        string level_texts = Level_Data_text.text;//�ؽ�Ʈ �����͸� ���ڿ��� �����´�
        string[] lines = level_texts.Split('\n');//���๮�ڸ��� �����Ͽ� ���ڿ� �迭�� �ִ´�.
        foreach(var line in lines)//lines ���� �� �࿡ ���Ͽ� ���ʷ� ó���� ���� ����
        {
            if (line == "")
            {
                continue;//���� �� ���̸� �Ʒ� ó���� ���� �ʰ� ���� ó������.
            };
            Debug.Log(line);//���� ������ ����� ���
            string[]words = line.Split();//�� ���� ���带 �迭�� ����
            int n = 0;

            LevelData level_data = new LevelData();//���� ó���ϴ� ���� �����͸� �־��.

            foreach(var word in words)
            {
                //words ���� �� ���忡 ���Ͽ� ������� ó��
                if (word.StartsWith("#"))
                {
                    break;//���� ���۹��ڰ� #�̸� ���� Ż��
                }
                if (word == "")
                {
                    continue;
                }

                switch (n)//n���� 0~7�� ��ȭ���Ѱ��� 8�׸�ó��. �� ���带 �÷԰����� ��ȯ �� ���������Ϳ� ����
                {
                    case 0:level_data.end_time = float.Parse(word); break;
                    case 1:level_data.player_speed = float.Parse(word); break;
                    case 2:level_data.floor_count.min = int.Parse(word);break;
                    case 3:level_data.floor_count.max = int.Parse(word);break;
                    case 4:level_data.hole_count.min = int.Parse(word);break;
                    case 5:level_data.hole_count.max = int.Parse(word);break;
                    case 6:level_data.height_diff.min = int.Parse(word);break;
                    case 7:level_data.height_diff.max = int.Parse(word);break;
                }
                n++;
            }
            if (n >= 8)
            {
                this.level_datas.Add(level_data);//8�׸� ���� ó�� �� ����Ʈ������ level_datas�� level_data�� �߰�
            }
            else
            {
                if (n == 0)
                {
                    //1 ���嵵 ó������ ���� ���=�ּ�-->����x
                }
                else
                {//�� �̿�-->������ ������ ���� �����Ƿ� �����޼��� ���
                    Debug.LogError("[LevelData] Out of parameter.\n");
                }
            }
        }
        if (this.level_datas.Count == 0)
        {
            Debug.LogError("[LevelData] Has no data\n");//level_datas�� �����Ͱ� �ϳ��� ������ �����޼��� ǥ��
            this.level_datas.Add(new LevelData());//�⺻ ���������͸� �ϳ� �߰�
        }
    }
}
