public enum ItemType
{
    披萨盒子,//0.5 40,0,0 book
    易拉罐, // 0.8 -100,90 can
    纸箱,  //0.65 30,0,0 book
    书,  // 0.6 70,180,0 book
    搪瓷水杯,  //0.6 -90,180,0 can 
    水壶,  // 0.6 90 metal
    电风扇,  // .5 -90 180 0 dianqi

    薯条, //0.7 90,0,0 fruit
    椰子壳,//0.8 40,0 ,0 wood
    汉堡, //0.5 -90,180,0 fruit 
    香蕉皮, //0.6 90,0,0  fruit
    牛油果,//0.6 -30,75,10 fruit
    西瓜皮,//0.6 170,-120,0 fruit 

    拖鞋, //0.6 -30,180,0 rzt 
    消防器,// 0.45 -115,-180,0 metal
    布娃娃,// 0.5 0,90,0 rubber
    塑料袋,//0.5 30,0,0 sld
    破背心,//0.5 -90,180,0 cloth
    篮球,  // 0.5 60,15,0 ball  

    游戏机, // 0.5 0 ,-90,-30 dianqi
    玻璃瓶, //0.6 -90,180,0 glass
    电池, //0.7 -60,-90,-90 dianchi 
    药品, //0.6 -80,-90,-90 pear 
    None,
}

public enum GarbageType
{
    回收垃圾,
    废弃垃圾,
    厨余垃圾,
    有害垃圾,
    变废为宝,
    None,
}

public enum StateType
{
    idle,
    open,
    enter,
    close,
    ready,
    refresh,
    None,
}

public enum FinishType
{
    成功,
    失败,
    没位置,
    时间到,
    None,
}
