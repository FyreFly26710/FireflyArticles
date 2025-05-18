import axios, { AxiosRequestConfig, AxiosResponse } from "axios";

// Dev uses https, Prod (docker) uses http
const DevBaseUrl = "https://localhost:21000";
const ProdBaseUrl = process.env.NEXT_PUBLIC_BASE_URL?.replace(/\/$/, '');
const url = ProdBaseUrl ?? DevBaseUrl;
// Default timeout (in milliseconds)
const DEFAULT_TIMEOUT = 20 * 1000;

// create Axios instance
const axiosInstance = axios.create({
    baseURL: url,
    timeout: DEFAULT_TIMEOUT,
    withCredentials: true,
});

// request interceptors
axiosInstance.interceptors.request.use(
    function (config) {
        if (!ProdBaseUrl) {
            // console.log("request interceptors: ", config);
        }
        return config;
    },
    function (error) {
        // error
        return Promise.reject(error);
    },
);

// response interceptors
axiosInstance.interceptors.response.use(
    // 2xx response
    // All api responses have 200 status code, including error responses
    function (response: AxiosResponse<any>) {
        return response.data;
    },
    // non 2xx error
    // these requests are not handled by the backend
    function (error) {
        return Promise.reject(error);
    },
);

// Type-safe request function
function request<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    // Use the provided timeout in config if available, otherwise use default
    const timeout = config?.timeout || DEFAULT_TIMEOUT;

    return axiosInstance.request<T, T>({
        url,
        ...config,
        timeout,
    });
}

// For backward compatibility with existing API usage
export default request;
export { url };