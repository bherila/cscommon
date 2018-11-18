using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Net.Sockets;
using System.Threading.Tasks;
using Nito.AsyncEx;

namespace EZ.Base {
	public static class Executor {

		private static T ExecWithTimeoutRetry<T>(Func<T> f) {
			var x = new Exception();
			var retry = 0;
			while (retry++ < 3)
				try {
					return f();
				} catch (SocketException ex) {
					x = ex;
				} catch (TimeoutException ex) {
					x = ex;
				}

			throw x;
		}

		/// <summary>
		///     Try to execute the selected function, handle validation problems, and return the negotiated result in the HTTP
		///     response.
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="f"></param>
		/// <returns></returns>
		public static ApiResult<T> ExecApi<T>(Func<T> f) {
			try {
				var result = ExecWithTimeoutRetry(f);
				return new ApiResult<T> {Success = true, Value = result};
			} catch (InvalidOperationException ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message + ex.StackTrace};
			} catch (ValidationException ve) {
				return new ApiResult<T> {Success = false, Message = ve.Message};
			} catch (KeyNotFoundException kx) {
				return new ApiResult<T> {Success = false, Message = kx.Message};
			} catch (ArgumentNullException ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message}.WithStack(ex.StackTrace);
			} catch (Exception ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message + (ex.InnerException?.Message ?? "")}
					.WithStack(ex.StackTrace);
			}
		}

		public static ApiResult<T> ExecApiAsync<T>(Func<Task<T>> f) {
			try {
				var result = AsyncContext.Run(f);
				return new ApiResult<T> {Success = true, Value = result};
			} catch (InvalidOperationException ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message + ex.StackTrace};
			} catch (ValidationException ve) {
				return new ApiResult<T> {Success = false, Message = ve.Message};
			} catch (KeyNotFoundException kx) {
				return new ApiResult<T> {Success = false, Message = kx.Message};
			} catch (ArgumentNullException ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message}.WithStack(ex.StackTrace);
			} catch (Exception ex) {
				return new ApiResult<T> {Success = false, Message = ex.Message + (ex.InnerException?.Message ?? "")}
					.WithStack(ex.StackTrace);
			}
		}
	}
}
