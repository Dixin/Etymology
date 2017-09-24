const HtmlWebpackPlugin = require("html-webpack-plugin");
const HtmlWebpackInlineSourcePlugin = require("html-webpack-inline-source-plugin");

const merge = require("webpack-merge");
const common = require("./webpack.config.js");

const Paths = common.Paths;
delete common.Paths;

module.exports = merge(common, {
    devtool: "eval-source-map",
    plugins: [
        new HtmlWebpackPlugin({
            template: Paths.indexTemplate,
            filename: Paths.indexHTML,
            inject: "body",
            showErrors: true
        })
    ]
});
