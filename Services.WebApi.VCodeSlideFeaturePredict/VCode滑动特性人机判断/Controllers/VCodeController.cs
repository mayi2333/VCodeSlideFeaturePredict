using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SSB.AI.VerificationCode.Models;

namespace VCode滑动特性人机判断.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class VCodeController : ControllerBase
    {
        /// <summary>
        /// 人机判断,同时记录下这次判断的参数和结果,用于后续训练模型
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [HttpPost]
        public JsonResult SlideFeaturePredict(VCodePredictFromModel model)
        {
            var m_result = new VCodePredictModel();
            m_result.code = 1;
            m_result.info = "调用成功";
            m_result.predic = SSB.AI.VerificationCode.Prediction.Prediction.Predict(model.GetObservationModel());
            Logs.WriteMessage(model.GetTestData(m_result.predic), "testdata");
            return new JsonResult(m_result);
        }
        //public JsonResult SlideFeaturePredict()
        //{
        //    var m_result = new VCodePredictModel();
        //    m_result.code = 1;
        //    m_result.info = "调用成功";
        //    m_result.predic = true;
        //    return new JsonResult(m_result);
        //    //m_result.predic = SSB.AI.VerificationCode.Prediction.Prediction.Predict(model);
        //}
    }
}
