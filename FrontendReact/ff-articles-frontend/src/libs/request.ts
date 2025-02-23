import { baseURL } from "@/config";
import axios, { AxiosResponse } from "axios";
const DevBaseUrl =  "http://localhost:21000/";
const TestBaseUrl = "https://demo.firefly-26710.com:8443"

// create Axios
const myAxios = axios.create({
    baseURL: TestBaseUrl,
    timeout: 20000,
    withCredentials: true,
});

// request interceptors
myAxios.interceptors.request.use(
    function (config) {
        // exec before request. do nothing now, can add like log
        console.log(config.baseURL)
        console.log(config.url)
        return config;
    },
    function (error) {
        // error
        return Promise.reject(error);
    },
);

// create interceptors
myAxios.interceptors.response.use(
    // 2xx response
    function (response: AxiosResponse<any>) {
        return response.data;
    },
    // non 2xx error
    function (error) {
        return Promise.reject(error);
    },
);

export default myAxios;