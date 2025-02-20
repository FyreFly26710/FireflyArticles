const { generateService } = require("@umijs/openapi");

const DEV_BASE_URL = "http://localhost:21000/";
const IDENTITY_API_URL = `${DEV_BASE_URL}identity/swagger/v1/swagger.json`;
const CONTENTS_API_URL = `${DEV_BASE_URL}contents/swagger/v1/swagger.json`;

// Generate services for Identity API
generateService({
    requestLibPath: "import request from '@/libs/request'",
    schemaPath: IDENTITY_API_URL,
    serversPath: "./src/api/identity",
});

// Generate services for Contents API
generateService({
    requestLibPath: "import request from '@/libs/request'",
    schemaPath: CONTENTS_API_URL,
    serversPath: "./src/api/contents",
});
