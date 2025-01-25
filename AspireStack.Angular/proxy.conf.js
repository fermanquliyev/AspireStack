module.exports = {
  "/api": {
    target:
      process.env["services__AspireStackApi__https__0"] ||
      process.env["services__AspireStackApi__http__0"],
    secure: process.env["NODE_ENV"] !== "development",
    pathRewrite: {
      "^/api": "",
    },
    headers: {
      Connection: "Keep-Alive",
    },
  },
};