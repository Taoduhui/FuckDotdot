# FuckDotdot

用于解决微信小程序typescript无法绝对路径引用的问题

Release: https://github.com/Taoduhui/FuckDotdot/releases/tag/v1.2.1

原理：

​	替换字符串，所以请注意alias的key不要和代码重复，否则会被替换，可以使在前加特殊符号的方式表示

使用方法：

1. 将FuckDotdot解压至tsconfig.json同级目录下

2. 配置FuckDotdot.xml

   ```xml
   <FuckDotdot>
     <AliasSettings>
       <GlobalAlias key="@ShiyiFramework">miniprogram/ShiyiFramework</GlobalAlias>
     </AliasSettings>
     <TargetFileTypes>
       <FileType type="*.js"></FileType>
       <FileType type="*.json">	
         <Alias key="@Npm">miniprogram/miniprogram_npm</Alias>
       </FileType>
       <FileType type="*.src.wxss" to="*.wxss">	
         <Alias key="@Npm">miniprogram/miniprogram_npm</Alias>
       </FileType>
     </TargetFileTypes>
   </FuckDotdot>
   ```

   注意：

   ​	如果是**源码ts中的引入，FileType的值应该是js**。

   ​	Alias的路径**没有 " ./ "**，末尾也**不加 " / "** 

   ​	正反斜杠无所谓

   ​	FileType中的to表示会将结果输出到带有新的扩展名的文件中

   ​	可以在.vscode/setting,json中通过添加如下代码将输出文件隐藏

   ```json
   {
       "files.exclude": {
           "**/*.wxss": {
               "when": "$(basename).src.wxss" //当存在.src.wxss同名文件时将.wxss隐藏
           }
       }
   }
   ```

   对于其他的配置了FileType文件同样有效，不限于js，但对于其他文件会直接替换掉Alias，请注意备份，以免误操作

3. tsconfig.json中添加paths

   ```json
   {
     "compilerOptions": {
       "baseUrl": "./",
       "paths": {
         "@ShiyiFramework/*":["./miniprogram/ShiyiFramework/*"]
       }
   }
   ```

4. 代码中引用

   ```typescript
   import { ShiyiPageBase } from "@ShiyiFramework/ShiyiPage/Base/ShiyiPageBase";
   ```

5. 将微信开发者工具的 详情 --> 本地设置 --> 自定义处理命令替换成以下命令

   ```
    npm run tsc && FuckWxDotdot.exe
   ```

6. 开始编译

7. 参数 **-w**  自动检测更改

