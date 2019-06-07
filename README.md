# (C#)滑块验证码滑动特性判断
一个ML.NET框架的验证码小演示
滑动验证码项目地址：https://github.com/eatage/VerificationCode

调用验证api部分代码是写再上面链接项目里的 这里贴出来

```
var code = HttpContext.Current.Session["code"].ToString();
var model = new Observation()
{
    sumtime = __totaldate,
    abscissa = float.Parse(code),
    total = _datalist.Length,
    meanv = (float)__mv,
    meanv1 = (float)__mv1,
    meanv2 = (float)__mv2,
    meanv3 = (float)__mv3,
    meana = (float)__ma,
    meana1 = (float)__ma1,
    meana2 = (float)__ma2,
    meana3 = (float)__ma3,
    standardv = (float)__o2v,
    standarda = (float)__o2a
};
string json = JsonConvert.SerializeObject(model);
string url = "http://localhost:11650/api/vcode/slidefeaturepredict";
string res = GetHttpWebResponse(url, json);
var remodel = JsonConvert.DeserializeObject<VCodePredictModel>(res);
if (remodel.code == 1 && remodel.predic)
{
    return true;
}
  
  
public class VCodePredictModel
{
    public int code;
    public string info;
    public bool predic;
}
public class Observation
{
    public float sumtime;
    public float abscissa;
    public float total;
    public float meanv;
    public float meanv1;
    public float meanv2;
    public float meanv3;
    public float meana;
    public float meana1;
    public float meana2;
    public float meana3;
    public float standardv;
    public float standarda;
    public bool label;
}
```
