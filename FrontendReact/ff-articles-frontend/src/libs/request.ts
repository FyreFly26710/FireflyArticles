import axios, { AxiosRequestConfig, AxiosResponse } from "axios";

const DevBaseUrl = "http://localhost:21000/";
const ProdBaseUrl = process.env.NEXT_PUBLIC_BASE_URL;
const url = ProdBaseUrl ?? DevBaseUrl;

// create Axios instance
const axiosInstance = axios.create({
    baseURL: url,
    timeout: 20000,
    withCredentials: true,
});

// request interceptors
axiosInstance.interceptors.request.use(
    function (config) {
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
    function (response: AxiosResponse<any>) {
        return response.data;
    },
    // non 2xx error
    function (error) {
        return Promise.reject(error);
    },
);

// Type-safe request function
function request<T = any>(url: string, config?: AxiosRequestConfig): Promise<T> {
    return axiosInstance.request<T, T>({
        url,
        ...config
    });
}

// For backward compatibility with existing API usage
export default request;