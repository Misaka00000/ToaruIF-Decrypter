# ToaruIF 解密工具使用说明

## 0 鸣谢

感谢**[Misaka00000](https://github.com/Misaka00000)**的开源项目。原项目似乎不再继续维护，故本项目在该版本基础上进行优化。

[Misaka00000项目]: https://github.com/Misaka00000/ToaruIF-Decrypter



## 1 概述

本工具旨在对某手游ToaruIF资源文件进行解密操作，仅供交流学习，不保证游戏更新后工具依然有效。

该手游资源加密方式大致如下：

- 偏移（Offset）
  - 偏移固定字节
  - 重复偏移固定字节
- 异或（Xor）
  - 单字节异或（0x00~0xFF）
  - 字符串异或（128bytes或256bytes字符串）

针对 *Offset* 类，读取文件头信息将多余字节舍弃。

针对 *Xor* 类单字节加密，识别字节并进行异或。

*针对 *Xor* 类字符串加密，需<kbd>keys.bin</kbd>文件。由于*2022.11.11*版本Unity更新修改了加密字符串（即所需的<kbd>keys.bin</kbd>需要更新），故该版本更新之后的资源文件需更新后的bin文件进行解密处理。

> 针对<kbd>keys.bin</kbd>的构成及原理不在本项目研究范畴，请自行解决。



## 2 本项目做出的优化

1. 优化完善了2大类解密方式的处理
2. 增加了目录迭代，输入目录可为多级目录
3. 针对异或类解密文件，可将解密后的文件再输入本程序可进行加密



## 3 使用方法

基本说明

```bash
ToaruIFDecrypter

Usage:
  ToaruIFDecrypter [options] [command]

Options:
  --version       Show version information
  -?, -h, --help  Show help and usage information

Commands:
  d, decrypt  Decrypt AssetBundles
```

使用默认<kbd>keys.bin</kbd>

```powershell
ToaruIFDecrypter.exe d -i <input floder> -o <output floder>
```

使用自己的<kbd>keys.bin</kbd>

```powershell
ToaruIFDecrypter.exe d -k <keys.bin> -i <input floder> -o <output floder>
```



- 建议可使用bat脚本简化工具使用流程。*（非必须）*

```bash
set rootpath="Your EXE root path"
set outpath="Your Output path"
set EXEname=ToaruIFDecrypter.exe
@echo off

cls
set input=:
set "input=%1"
:: 上面这句为判断%input%中是否存在引号，有则剔除。
if not exist %outpath% md %outpath%
if "%input%"==":" goto input
if not exist "%input%" goto input
cd "%rootpath%"
start "" /b /w "%rootpath%\%EXEname%" d -i "%input%" -o "%outpath%"

pause
```

保存为<kbd>xxx.bat</kbd>（xxx为任意文件名），并将需解密的文件夹拖入该脚本即可实现解密。

