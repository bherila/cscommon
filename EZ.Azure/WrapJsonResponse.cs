using System.Net;
using System.Net.Http;
using EZ.Base;
using Newtonsoft.Json;

namespace EZ.Azure {
	public static class WrapJsonResponse {
		public static HttpResponseMessage ToJsonHttpResponseMessage<T>(this ApiResult<T> apiResult) {
			return new HttpResponseMessage(HttpStatusCode.OK) {Content = new StringContent(JsonConvert.SerializeObject(apiResult))};
		}
	}
}
