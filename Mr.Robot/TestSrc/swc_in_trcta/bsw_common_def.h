/*	$RCSfile: bsw_common_def.h $									*/
/*	$Date: 2015/11/10 13:56:27JST $									*/
/*	$Revision: 1.4 $												*/
/*	 EXPLANATION: BSW���ʒ�` ���J�w�b�_�t�@�C��					*/

#ifndef BSW_COMMON_DEF_INC
#define BSW_COMMON_DEF_INC

#ifndef BSW_COMMON_DEF_DEF
#define BSW_COMMON_DEF_EXT extern
#else
#define BSW_COMMON_DEF_EXT
#endif

/*	���͐M���̃f�[�^��ԋ��ʒ�`	*/
#define IN_MSGSTS_NONE				(CSMON_MSGSTS_NONE)			/*	����							*/
#define IN_MSGSTS_NORX				(CSMON_MSGSTS_NORX)			/*	����M							*/
#define IN_MSGSTS_TIMEOUT			(CSMON_MSGSTS_TIMEOUT)		/*	�^�C���A�E�g����				*/
#define IN_MSGSTS_TXSTOP			(CSMON_MSGSTS_TXSTOP)		/*	���M�ꎞ��~���				*/
#define IN_MSGSTS_FAILINIT			(CSMON_MSGSTS_FAIL)			/*	�r��							*/
#define IN_MSGSTS_NG				(CSMON_MSGSTS_NG)			/*	�ُ���						*/

/*	�����l�̃f�[�^��ԋ��ʒ�`	*/
#define PV_STS_NORMAL				((uint8) 0U)				/*	����							*/
#define PV_STS_SHORT				((uint8) 1U)				/*	�V���[�g						*/
#define PV_STS_TIMEOUT				((uint8) 2U)				/*	�^�C���A�E�g					*/
#define PV_STS_RNGOVR				((uint8) 3U)				/*	�����W�I�[�o�[					*/
#define PV_STS_INVALID				((uint8) 4U)				/*	����							*/
#define PV_STS_JUDGING				((uint8) 5U)				/*	�G���[���蒆					*/
#define PV_STS_FAIL					((uint8) 6U)				/*	�r��							*/
#define PV_STS_NOTRCV				((uint8) 7U)				/*	����M							*/
#define PV_STS_ERR					((uint8) 8U)				/*	�ُ�							*/
#define PV_STS_UNKNOWN				((uint8) 9U)				/*	�s��							*/

/*	AD�l�̃f�[�^��ԋ��ʒ�`	*/
#define AD_STS_NORMAL				((uint8) 0U)				/*	����							*/
#define AD_STS_ABNORMAL				((uint8) 1U)				/*	�ُ�							*/
#define AD_STS_JUDGING				((uint8) 2U)				/*	�G���[��Ԕ��蒆				*/
#define AD_STS_UNKNOWN				((uint8) 255U)				/*	���m��							*/

/*	����ڕW�l�̃f�[�^��ԋ��ʒ�`	*/
#define TARGET_STS_NORMAL			((uint8) 0U)				/*	����							*/
#define TARGET_STS_FAIL				((uint8) 1U)				/*	�r��							*/
#define TARGET_STS_UNKNOWN			((uint8) 2U)				/*	���m��							*/

/*	�i���f�[�^�Ǐo�����ʒ�`	*/
#define NV_READ_SUCCESS				((uint8) 0U)				/*	�Ǐo������						*/
#define NV_READ_FAIL				((uint8) 16U)				/*	�Ǐo�����s						*/
#define NV_READ_NOTACCEPT			((uint8) 17U)				/*	�Ǐo����t�s��					*/
#define NV_READ_INVALID				((uint8) 255U)				/*	�����ُ�						*/

/*	�i���f�[�^�����݌��ʒ�`	*/
#define NV_WRITE_SUCCESS			((uint8) 0U)				/*	�����ݐ���						*/
#define NV_WRITE_ACCEPT				((uint8) 1U)				/*	�����ݎ�t						*/
#define NV_WRITE_FAIL				((uint8) 16U)				/*	�����ݎ��s						*/
#define NV_WRITE_NOTACCEPT			((uint8) 17U)				/*	�����ݎ�t�s��					*/
#define NV_WRITE_INVALID			((uint8) 255U)				/*	�����ُ�						*/

/*	�i���f�[�^�����ݏ�Ԓ�`	*/
#define NV_STS_NORMAL				((uint8) 0U)				/*	�ʏ�(�A�C�h��)					*/
#define NV_STS_PREINIT				((uint8) 1U)				/*	�������O						*/
#define NV_STS_WRITING				((uint8) 2U)				/*	�����ݒ�(�󂯕t������)			*/
#define NV_STS_WRITE_NOISEERR		((uint8) 4U)				/*	�����݃G���[(NOISE)				*/
#define NV_STS_WRITE_HWERR			((uint8) 8U)				/*	�����݃G���[(HW)				*/
#define NV_STS_READ_ERR				((uint8) 16U)				/*	�ǂݏo���G���[					*/
#define NV_STS_INVALID				((uint8) 255U)				/*	�f�[�^��Ԗ���					*/

/*	�^�C�}�̃f�[�^��ԋ��ʒ�`	*/
#define TM_TIMSTS_STOP				((uint8) 0U)				/*	�^�C�}��~��					*/
#define TM_TIMSTS_START				((uint8) 1U)				/*	�^�C�}���쒆					*/
#define TM_TIMSTS_TIMEOUT			((uint8) 16U)				/*	�^�C���A�E�g��					*/

/*	�^�C�}�̎�ʋ��ʒ�`	*/
#define TM_TIMTYP_ONESHOT			((uint8) 0U)				/*	�����V���b�g�^�C�}(�����Ȃ�)	*/
#define TM_TIMTYP_CYCLIC			((uint8) 1U)				/*	�T�C�N���b�N�^�C�}(�����Ȃ�)	*/
#define TM_TIMTYP_SYNC_ONE			((uint8) 16U)				/*	�����V���b�g�^�C�}(��������)	*/
#define TM_TIMTYP_SYNC_CYC			((uint8) 17U)				/*	�T�C�N���b�N�^�C�}(��������)	*/

/*	�X�^���o�C�ڍs���茋�ʂ̋��ʒ�`	*/
#ifndef	STANDBY_CHK_SLEEP_OK
#define STANDBY_CHK_SLEEP_OK		((uint8) 0U)				/*	�X���[�vOK						*/
#endif	/* STANDBY_CHK_SLEEP_OK */
#ifndef	STANDBY_CHK_SLEEP_NG
#define STANDBY_CHK_SLEEP_NG		((uint8) 1U)				/*	�X���[�vNG						*/
#endif	/* STANDBY_CHK_SLEEP_NG */

/*	�R�l�N�^�[IG�d�� 7V����	*/
#ifndef	RTE_MODE_IG7V_OFF
#define RTE_MODE_IG7V_OFF			((uint8) 0U)				/*	7V����							*/
#endif	/* RTE_MODE_IG7V_OFF */
#ifndef	RTE_MODE_IG7V_ON
#define RTE_MODE_IG7V_ON			((uint8) 1U)				/*	7V�ȏ�							*/
#endif	/* RTE_MODE_IG7V_ON */
#ifndef	RTE_MODE_IG7V_UNKNOWN
#define RTE_MODE_IG7V_UNKNOWN		((uint8) 2U)				/*	�s��							*/
#endif	/* RTE_MODE_IG7V_UNKNOWN */

/*	�R�l�N�^�[IG�d�� 3.8V����	*/
#ifndef	RTE_MODE_IG3D8V_OFF
#define RTE_MODE_IG3D8V_OFF			((uint8) 0U)				/*	3.8V����						*/
#endif	/* RTE_MODE_IG3D8V_OFF */
#ifndef	RTE_MODE_IG3D8V_ON
#define RTE_MODE_IG3D8V_ON			((uint8) 1U)				/*	3.8V�ȏ�						*/
#endif	/* RTE_MODE_IG3D8V_ON */
#ifndef	RTE_MODE_IG3D8V_UNKNOWN
#define RTE_MODE_IG3D8V_UNKNOWN		((uint8) 2U)				/*	�s��							*/
#endif	/* RTE_MODE_IG3D8V_UNKNOWN */

/*	IG�d��A/D�l	*/
#define IN_IGV_AD_MIN				((uint16) 0U)				/*	IG�d��A/D�l�ŏ��l				*/
#define IN_IGV_AD_MAX				((uint16) 1023U)			/*	IG�d��A/D�l�ő�l				*/

/*	ACC���	*/
#ifndef	RTE_MODE_ACC_OFF
#define RTE_MODE_ACC_OFF			((uint8) 0U)				/*	ACC OFF							*/
#endif	/* RTE_MODE_ACC_OFF */
#ifndef	RTE_MODE_ACC_ON
#define RTE_MODE_ACC_ON				((uint8) 1U)				/*	ACC ON							*/
#endif	/* RTE_MODE_ACC_ON */

/*	�R�l�N�^�[IG�d�� 10V����	*/
#ifndef	RTE_MODE_IG10V_OFF
#define RTE_MODE_IG10V_OFF			((uint8) 0U)				/*	10V����							*/
#endif	/* RTE_MODE_IG10V_OFF */
#ifndef	RTE_MODE_IG10V_ON
#define RTE_MODE_IG10V_ON			((uint8) 1U)				/*	10V�ȏ�							*/
#endif	/* RTE_MODE_IG10V_ON */
#ifndef	RTE_MODE_IG10V_UNKNOWN
#define RTE_MODE_IG10V_UNKNOWN		((uint8) 2U)				/*	�s��							*/
#endif	/* RTE_MODE_IG10V_UNKNOWN */

/*	�R�l�N�^�[IG�d�� 10.5V����	*/
#ifndef	RTE_MODE_IG10D5V_OFF
#define RTE_MODE_IG10D5V_OFF		((uint8) 0U)				/*	10.5V����						*/
#endif	/* RTE_MODE_IG10D5V_OFF */
#ifndef	RTE_MODE_IG10D5V_ON
#define RTE_MODE_IG10D5V_ON			((uint8) 1U)				/*	10.5V�ȏ�						*/
#endif	/* RTE_MODE_IG10D5V_ON */
#ifndef	RTE_MODE_IG10D5V_UNKNOWN
#define RTE_MODE_IG10D5V_UNKNOWN	((uint8) 2U)				/*	�s��							*/
#endif	/* RTE_MODE_IG10D5V_UNKNOWN */

#endif	/* BSW_COMMON_DEF_INC */

