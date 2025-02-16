import { Result, Button } from "antd";
import React from "react";

/**
 * No access permission
 * @constructor
 */
const Forbidden = () => {
    return (
        <Result
            status="403"
            title="403"
            subTitle="Sorry, you do not have permission to access this page."
            extra={
                <Button type="primary" href="/">
                    Return to Homepage
                </Button>
            }
        />
    );
};

export default Forbidden;
