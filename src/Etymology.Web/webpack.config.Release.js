const webpack = require("webpack");
const HtmlWebpackPlugin = require("html-webpack-plugin");
const HtmlWebpackInlineSourcePlugin = require("html-webpack-inline-source-plugin");

const merge = require("webpack-merge");
const common = require("./webpack.config.js");

const Paths = common.Paths;
delete common.Paths;

module.exports = merge(common, {
    plugins: [
        new HtmlWebpackPlugin({
            template: Paths.indexTemplate,
            filename: Paths.indexHTML,
            inlineSource: ".js$",
            inject: "body",
            showErrors: true,
            minify: {
                removeComments: true,
                collapseWhitespace: true
            }
        }),
        new webpack.optimize.UglifyJsPlugin({
            comments: false
        }),
        new HtmlWebpackInlineSourcePlugin()
    ]
});
