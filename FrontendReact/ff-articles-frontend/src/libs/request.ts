import axios from "axios";
import { baseURL } from "@/config";

const DevBaseUrl =  "http://localhost:21000/";
const TestBaseUrl = baseURL

// create Axios
const myAxios = axios.create({
    baseURL: DevBaseUrl,
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
    function (response) {
        // response data
        const { data } = response;
        // not logged in error
        if (data.code === 40100) {
            // redirect to login page
            // if (
            //     !response.request.responseURL.includes("user/get/login") &&
            //     !window.location.pathname.includes("/user/login")
            // ) {
            //     window.location.href = `/user/login?redirect=${window.location.href}`;
            // }
        } else if (data.code !== 0) {
            // other errors
            throw new Error(data.message ?? "system error");
        }
        return data;
    },
    // non 2xx error
    function (error) {
        return Promise.reject(error);
    },
);

export default myAxios;
