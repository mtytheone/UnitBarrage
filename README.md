# UnitBarrage

[![ver:1.1.2](https://img.shields.io/badge/ver-1.1.2-8181F7.svg)](https://github.com/mtytheone/UnitBarrage/releases/tag/v1.1.2)
[![License:MIT](https://img.shields.io/badge/License-MIT-04B431.svg)](https://choosealicense.com/licenses/mit/)

## About - 概要
UnityのPreview版PackageであるECSを使用した簡単な弾幕シューティング系のミニゲームです。タイトル画面は存在しておらず、設定画面とゲーム本体のみで構成されています。全3段階の攻撃を備えたボスが一体出現します。加えて、プレイヤーの残り残機とボス撃破の素早さを甘味して集計されるオンラインでのスコアランキングがあります（オンラインモードのみ）。

## Video - プレイ映像
以下の画像リンク先のYoutubeにてご覧いただけます。
[![ゲームイメージ](https://img.youtube.com/vi/hGCitr9zpgU/0.jpg)](https://www.youtube.com/watch?v=hGCitr9zpgU)

## Approval - 同意
ゲームを起動した時点で、以下の文面のいずれかを選択したと見なします。

このゲームには、オンラインスコアランキングを実装しています。

ゲーム終了時に設定画面にて記入した名前およびスコア、識別ID、時刻をデータベースに送信します。このデータベースとのやり取りにおいて、IPアドレス等の個人を識別できる情報を送信することは一切ありません。

これらに関して、あらかじめご了承いただける方は、そのままゲームを遊んでいただければと思います。

同意いただけない方に関しましては、設定画面の下にオフラインモードで遊ぶことが出来るチェックボックスがありますので、そちらを選択した状態でPlayボタンを押していただければ通信することなくゲームが遊べますので、そちらをご利用いただければと思います。

## Hou to use - 起動方法
[Releaseページ](https://github.com/mtytheone/UnitBarrage/releases/)からZipファイルをダウンロードして**UnitBarrage.exe**ファイルを起動すると遊べます。

## How to operate - 操作方法
キーボードとPS4コントローラーの2種類で遊ぶことが出来ます。
### Keyboard - キーボード
- Z キー or Spaceキー or Enterキー : 攻撃
- WASD キー or 矢印キー : 移動
- Shift キー : 低速移動
- Escキー : ポーズ
### DUALSHOCK - PS4コントローラー
- R2トリガー : 攻撃
- Lスティック : 移動
- L2トリガー : 低速移動
- Optionボタン : ポーズ

## Tips - ちょっとした仕様
- HPチャージ開始から5秒後にボスが弾を発射してきます。
- 被弾後のクールタイムはありません。
- プレイヤーの当たり判定は真ん中の赤い部分のみになります。
- プレイヤーの被弾部（赤い丸）さえ当たらなければ、ボスにめり込んでも大丈夫です。
- 別アプリを表示した場合、ゲーム中ではポーズになります。

## License - ライセンス
このゲーム、プロジェクト及びソースファイルは **MIT** ライセンスに乗っ取ります。

- [ライセンス原文](https://github.com/mtytheone/UnitBarrage/blob/master/LICENSE.md)

- [ライセンスページ](https://choosealicense.com/licenses/mit/)

## Production Environment - 制作環境
- **Unity** 2020.1.4f1
- **UniversalRenderingPipeline** ver.8.2.0
- **HavokPhysicsforUnity** ver.0.4.0-preview.1
    - **Burst** ver.1.3.2
    - **Collections** ver.0.12.0-preview.13
    - **Entities** ver.0.14.0-preview.18
    - **Jobs** ver.0.5.0-preview.14
    - **Unity Physics** ver.0.5.0-preview.1
- **Hybrid Renderer** ver.0.8.0-preview.18
- **InputSystem** ver.1.0.0
- **TextMeshPro** ver.3.0.1

## About Project - プロジェクトに関して
プライバシーの関係上、フォントとGSSSettingについてはGitにコミットしていません。そのため、起動して確かめる際にはオフラインモードのみ正常に動くと思います。
Cloneして使う際は、GameScene内のTextに、TextMeshProに日本語対応したフォントを作成したものをアタッチしてください。

加えて、プロジェクト内にて不備などがありましたら、[Twitter](https://twitter.com/kohu_vr)にてリプを飛ばしてもらえればと思います。

## Tester - テスター
- ふぁるこ [@faruco10032](https://twitter.com/faruco10032)
- がとーしょこら [@gatosyocora](https://twitter.com/gatosyocora)
- ミツキツネ [@mitsu_kitsune](https://twitter.com/mitsu_kitsune)
- 胡椒少々塩少佐 [@syousa2003](https://twitter.com/syousa2003)
- かんざきちゃん [@kanzakich_vr](https://twitter.com/kanzakich_vr)