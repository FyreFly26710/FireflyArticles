import axios, { AxiosResponse } from "axios";
const DevBaseUrl = "http://localhost:21000/";
const ProdBaseUrl = process.env.NEXT_PUBLIC_BASE_URL;

// create Axios
const myAxios = axios.create({
    baseURL: DevBaseUrl,
    timeout: 20000,
    withCredentials: true,
});

// request interceptors
myAxios.interceptors.request.use(
    function (config) {
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