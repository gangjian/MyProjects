/*	$RCSfile: bsw_common_def.h $									*/
/*	$Date: 2015/11/10 13:56:27JST $									*/
/*	$Revision: 1.4 $												*/
/*	 EXPLANATION: BSW共通定義 公開ヘッダファイル					*/

#ifndef BSW_COMMON_DEF_INC
#define BSW_COMMON_DEF_INC

#ifndef BSW_COMMON_DEF_DEF
#define BSW_COMMON_DEF_EXT extern
#else
#define BSW_COMMON_DEF_EXT
#endif

/*	入力信号のデータ状態共通定義	*/
#define IN_MSGSTS_NONE				(CSMON_MSGSTS_NONE)			/*	正常							*/
#define IN_MSGSTS_NORX				(CSMON_MSGSTS_NORX)			/*	未受信							*/
#define IN_MSGSTS_TIMEOUT			(CSMON_MSGSTS_TIMEOUT)		/*	タイムアウト発生				*/
#define IN_MSGSTS_TXSTOP			(CSMON_MSGSTS_TXSTOP)		/*	送信一時停止状態				*/
#define IN_MSGSTS_FAILINIT			(CSMON_MSGSTS_FAIL)			/*	途絶							*/
#define IN_MSGSTS_NG				(CSMON_MSGSTS_NG)			/*	異常状態						*/

/*	物理値のデータ状態共通定義	*/
#define PV_STS_NORMAL				((uint8) 0U)				/*	正常							*/
#define PV_STS_SHORT				((uint8) 1U)				/*	ショート						*/
#define PV_STS_TIMEOUT				((uint8) 2U)				/*	タイムアウト					*/
#define PV_STS_RNGOVR				((uint8) 3U)				/*	レンジオーバー					*/
#define PV_STS_INVALID				((uint8) 4U)				/*	無効							*/
#define PV_STS_JUDGING				((uint8) 5U)				/*	エラー判定中					*/
#define PV_STS_FAIL					((uint8) 6U)				/*	途絶							*/
#define PV_STS_NOTRCV				((uint8) 7U)				/*	未受信							*/
#define PV_STS_ERR					((uint8) 8U)				/*	異常							*/
#define PV_STS_UNKNOWN				((uint8) 9U)				/*	不定							*/

/*	AD値のデータ状態共通定義	*/
#define AD_STS_NORMAL				((uint8) 0U)				/*	正常							*/
#define AD_STS_ABNORMAL				((uint8) 1U)				/*	異常							*/
#define AD_STS_JUDGING				((uint8) 2U)				/*	エラー状態判定中				*/
#define AD_STS_UNKNOWN				((uint8) 255U)				/*	未確定							*/

/*	制御目標値のデータ状態共通定義	*/
#define TARGET_STS_NORMAL			((uint8) 0U)				/*	正常							*/
#define TARGET_STS_FAIL				((uint8) 1U)				/*	途絶							*/
#define TARGET_STS_UNKNOWN			((uint8) 2U)				/*	未確定							*/

/*	永続データ読出し結果定義	*/
#define NV_READ_SUCCESS				((uint8) 0U)				/*	読出し成功						*/
#define NV_READ_FAIL				((uint8) 16U)				/*	読出し失敗						*/
#define NV_READ_NOTACCEPT			((uint8) 17U)				/*	読出し受付不可					*/
#define NV_READ_INVALID				((uint8) 255U)				/*	引数異常						*/

/*	永続データ書込み結果定義	*/
#define NV_WRITE_SUCCESS			((uint8) 0U)				/*	書込み成功						*/
#define NV_WRITE_ACCEPT				((uint8) 1U)				/*	書込み受付						*/
#define NV_WRITE_FAIL				((uint8) 16U)				/*	書込み失敗						*/
#define NV_WRITE_NOTACCEPT			((uint8) 17U)				/*	書込み受付不可					*/
#define NV_WRITE_INVALID			((uint8) 255U)				/*	引数異常						*/

/*	永続データ書込み状態定義	*/
#define NV_STS_NORMAL				((uint8) 0U)				/*	通常(アイドル)					*/
#define NV_STS_PREINIT				((uint8) 1U)				/*	初期化前						*/
#define NV_STS_WRITING				((uint8) 2U)				/*	書込み中(受け付けから)			*/
#define NV_STS_WRITE_NOISEERR		((uint8) 4U)				/*	書込みエラー(NOISE)				*/
#define NV_STS_WRITE_HWERR			((uint8) 8U)				/*	書込みエラー(HW)				*/
#define NV_STS_READ_ERR				((uint8) 16U)				/*	読み出しエラー					*/
#define NV_STS_INVALID				((uint8) 255U)				/*	データ状態無効					*/

/*	タイマのデータ状態共通定義	*/
#define TM_TIMSTS_STOP				((uint8) 0U)				/*	タイマ停止中					*/
#define TM_TIMSTS_START				((uint8) 1U)				/*	タイマ動作中					*/
#define TM_TIMSTS_TIMEOUT			((uint8) 16U)				/*	タイムアウト済					*/

/*	タイマの種別共通定義	*/
#define TM_TIMTYP_ONESHOT			((uint8) 0U)				/*	ワンショットタイマ(同期なし)	*/
#define TM_TIMTYP_CYCLIC			((uint8) 1U)				/*	サイクリックタイマ(同期なし)	*/
#define TM_TIMTYP_SYNC_ONE			((uint8) 16U)				/*	ワンショットタイマ(同期あり)	*/
#define TM_TIMTYP_SYNC_CYC			((uint8) 17U)				/*	サイクリックタイマ(同期あり)	*/

/*	スタンバイ移行判定結果の共通定義	*/
#ifndef	STANDBY_CHK_SLEEP_OK
#define STANDBY_CHK_SLEEP_OK		((uint8) 0U)				/*	スリープOK						*/
#endif	/* STANDBY_CHK_SLEEP_OK */
#ifndef	STANDBY_CHK_SLEEP_NG
#define STANDBY_CHK_SLEEP_NG		((uint8) 1U)				/*	スリープNG						*/
#endif	/* STANDBY_CHK_SLEEP_NG */

/*	コネクタ端IG電圧 7V判定	*/
#ifndef	RTE_MODE_IG7V_OFF
#define RTE_MODE_IG7V_OFF			((uint8) 0U)				/*	7V未満							*/
#endif	/* RTE_MODE_IG7V_OFF */
#ifndef	RTE_MODE_IG7V_ON
#define RTE_MODE_IG7V_ON			((uint8) 1U)				/*	7V以上							*/
#endif	/* RTE_MODE_IG7V_ON */
#ifndef	RTE_MODE_IG7V_UNKNOWN
#define RTE_MODE_IG7V_UNKNOWN		((uint8) 2U)				/*	不定							*/
#endif	/* RTE_MODE_IG7V_UNKNOWN */

/*	コネクタ端IG電圧 3.8V判定	*/
#ifndef	RTE_MODE_IG3D8V_OFF
#define RTE_MODE_IG3D8V_OFF			((uint8) 0U)				/*	3.8V未満						*/
#endif	/* RTE_MODE_IG3D8V_OFF */
#ifndef	RTE_MODE_IG3D8V_ON
#define RTE_MODE_IG3D8V_ON			((uint8) 1U)				/*	3.8V以上						*/
#endif	/* RTE_MODE_IG3D8V_ON */
#ifndef	RTE_MODE_IG3D8V_UNKNOWN
#define RTE_MODE_IG3D8V_UNKNOWN		((uint8) 2U)				/*	不定							*/
#endif	/* RTE_MODE_IG3D8V_UNKNOWN */

/*	IG電圧A/D値	*/
#define IN_IGV_AD_MIN				((uint16) 0U)				/*	IG電圧A/D値最小値				*/
#define IN_IGV_AD_MAX				((uint16) 1023U)			/*	IG電圧A/D値最大値				*/

/*	ACC情報	*/
#ifndef	RTE_MODE_ACC_OFF
#define RTE_MODE_ACC_OFF			((uint8) 0U)				/*	ACC OFF							*/
#endif	/* RTE_MODE_ACC_OFF */
#ifndef	RTE_MODE_ACC_ON
#define RTE_MODE_ACC_ON				((uint8) 1U)				/*	ACC ON							*/
#endif	/* RTE_MODE_ACC_ON */

/*	コネクタ端IG電圧 10V判定	*/
#ifndef	RTE_MODE_IG10V_OFF
#define RTE_MODE_IG10V_OFF			((uint8) 0U)				/*	10V未満							*/
#endif	/* RTE_MODE_IG10V_OFF */
#ifndef	RTE_MODE_IG10V_ON
#define RTE_MODE_IG10V_ON			((uint8) 1U)				/*	10V以上							*/
#endif	/* RTE_MODE_IG10V_ON */
#ifndef	RTE_MODE_IG10V_UNKNOWN
#define RTE_MODE_IG10V_UNKNOWN		((uint8) 2U)				/*	不定							*/
#endif	/* RTE_MODE_IG10V_UNKNOWN */

/*	コネクタ端IG電圧 10.5V判定	*/
#ifndef	RTE_MODE_IG10D5V_OFF
#define RTE_MODE_IG10D5V_OFF		((uint8) 0U)				/*	10.5V未満						*/
#endif	/* RTE_MODE_IG10D5V_OFF */
#ifndef	RTE_MODE_IG10D5V_ON
#define RTE_MODE_IG10D5V_ON			((uint8) 1U)				/*	10.5V以上						*/
#endif	/* RTE_MODE_IG10D5V_ON */
#ifndef	RTE_MODE_IG10D5V_UNKNOWN
#define RTE_MODE_IG10D5V_UNKNOWN	((uint8) 2U)				/*	不定							*/
#endif	/* RTE_MODE_IG10D5V_UNKNOWN */

#endif	/* BSW_COMMON_DEF_INC */

