using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Model : MonoBehaviour {

    public const int NORMAL_ROWS = 20;
    public const int MAX_ROWS = 23;         //最大行数
    public const int MAX_COLUMNS = 10;      //最大列数

    private Transform[,] map = new Transform[MAX_COLUMNS, MAX_ROWS];

    private int score = 0;                  //当前排名
    private int highScore =0;               //最高成绩
    private int numbersGame = 0;            //排名

    public int Score { get { return score; } }
    public int HighScore { get { return highScore; } }
    public int NumbersGame { get { return numbersGame; } }

    public bool isDataUpdate = false;

    private void Awake()
    {
        LoadData();
    }
    /// <summary>
    /// 判断是否在地图内
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool IsValidMapPosition(Transform t)
    {
        foreach(Transform child in t)
        {
            if (child.tag != "Block") continue;     //过滤锚点pivot子对象
            Vector2 pos = child.position.Round();
            if (IsInsideMap(pos) == false) return false;
            if (map[(int)pos.x, (int)pos.y] != null)    //是否跟别的方块重叠
                return false;
        }
        return true;
    }
    public bool IsGameOver()
    {
        for(int i = NORMAL_ROWS; i < MAX_ROWS; i++)
        {   //行数
            for ( int j = 0; j < MAX_COLUMNS; j++)
            {       //列数
                if (map[j, i] != null)  //在地图以上还有方块，则游戏结束
                {
                    numbersGame++;
                    SaveData();
                    return true;
                }
            }
        }
        return false;
    }
    /// <summary>
    /// 是否超出边界
    /// </summary>
    /// <param name="pos"></param>
    /// <returns></returns>
    private bool IsInsideMap(Vector2 pos)
    {
        //分别是左边界    右边界    下边界
        return pos.x >= 0 && pos.x < MAX_COLUMNS && pos.y >= 0;
    }
    /// <summary>
    /// 把落下的方块塞到map数组里
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public bool PlaceShape(Transform t)
    {
        foreach(Transform child in t)
        {
            //上一个落下的方块
            if (child.tag != "Block") continue;
            Vector2 pos = child.position.Round();
            map[(int)pos.x, (int)pos.y] = child;    //把地图上的该位置设置为有对象，下个对象就不会重叠
        }
        return CheckMap();  //如果消除行数大于0，就执行消除音效
    }
    //检查地图是否不要消除行
    private bool CheckMap()
    {
        int count = 0;  //消除行数
        for (int i =0;i< MAX_ROWS;i++)
        {
            bool isFull = CheckIsRowFull(i);
            if (isFull)
            {
                count++;
                DeleteRow(i);
                MoveDownRowsAbove(i + 1);   //把上一行往下移
                i--;                        //如果多行已满的情况，把本行及以上消除
            }
        }
        if (count > 0)  //如果消除行数大于0，就执行消除音效
        {
            score += (count * 100); //得分
            if (score > highScore)
            {       //最高得分更新
                highScore = score;
            }
            isDataUpdate = true;
            return true;
        }
        else return false;
    }
    private bool CheckIsRowFull(int row)
    {
        for(int i = 0; i < MAX_COLUMNS; i++)
        {
            if (map[i, row] == null) return false;
        }
        return true;
    }
    //删除当前行
    private void DeleteRow(int row)
    {
        for (int i = 0; i < MAX_COLUMNS; i++)
        {
            Destroy(map[i, row].gameObject);
            map[i, row] = null;
        }
    }
    /// <summary>
    /// 把当前行移动到最上层
    /// </summary>
    /// <param name="row"></param>
    private void MoveDownRowsAbove(int row)
    {
        for(int i = row; i < MAX_ROWS; i++)
        {
            MoveDownRow(i);     //本行，以及本行以上都往下移
        }
    }
    private void MoveDownRow(int row)
    {
        for( int i= 0; i < MAX_COLUMNS; i++)
        {
            if(map[i, row] != null)
            {
                map[i, row - 1] = map[i, row];  //把当前的行内容保存到下一行的内容（这样保持没有）
                map[i, row] = null;     //清除本行内容
                map[i, row - 1].position += new Vector3(0, -1, 0);//所有行（非本行）都往下移
            }
        }
    }
    /// <summary>
    /// 加载初始分和最高分
    /// </summary>
    private void LoadData()
    {
        highScore = PlayerPrefs.GetInt("HighScore", 0);//第二个是初始值
        numbersGame = PlayerPrefs.GetInt("NumbersGame", 0);
    }
    /// <summary>
    /// 保存初始分和最高分
    /// </summary>
    private void SaveData()
    {
        PlayerPrefs.SetInt("HighScore", highScore);
        PlayerPrefs.SetInt("NumbersGame", numbersGame);
    }
    public void Restart()
    {
        for(int i = 0; i < MAX_COLUMNS; i++)
        {
            for(int j = 0; j < MAX_ROWS; j++)
            {
                if (map[i, j] != null)
                {
                    Destroy(map[i, j].gameObject);
                    map[i, j] = null;
                }
            }
        }
        score = 0;
    }
    /// <summary>
    /// 清除数据
    /// </summary>
    public void ClearData()
    {
        score = 0;
        highScore = 0;
        numbersGame = 0;
        SaveData();
    }
}
