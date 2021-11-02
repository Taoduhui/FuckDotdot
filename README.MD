# FuckDotdot

用于解决微信小程序typescript无法绝对路径引用的问题

原理：

​	替换字符串，所以请注意alias的key不要和代码重复，否则会被替换，可以使在前加特殊符号的方式表示

使用方法：

1. 将FuckDotdot解压至tsconfig.json同级目录下

2. 配置FuckDotdot.xml

   ```xml
   <FuckDotdot>
     <AliasSettings>
       <Alias key="@ShiyiFramework">miniprogram\ShiyiFramework</Alias>
     </AliasSettings>
     <TargetFileTypes>
       <FileType>*.js</FileType>
     </TargetFileTypes>
   </FuckDotdot>
   ```

   注意：

   ​	如果是**源码ts中的引入，FileType的值应该是js**。

   ​	Alias的路径**没有 " ./ "**，末尾也**不加 " / "** 

   ​	正反斜杠无所谓

   对于其他的配置了FileType文件同样有效，不限

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
