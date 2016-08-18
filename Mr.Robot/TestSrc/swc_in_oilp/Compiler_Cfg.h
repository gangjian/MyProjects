/******************************************************************************
 *
 *                    Mentor Graphics Corporation
 *                        All rights reserved
 *
 ******************************************************************************
 *
 * Module:       Common - Compiler Abstraction
 *
 * File Name:    Compiler_Cfg.h
 *
 * Description:  This file contains definitions for different memory classes
 *               and pointer classes for different modules.
 *
 *
 ******************************************************************************/

#ifndef COMPILER_CFG_H
#define COMPILER_CFG_H

/*****************************************************************************/
/* Included standard header files                                            */
/*****************************************************************************/

/*****************************************************************************/
/* Included other header files                                               */
/*****************************************************************************/
/*#include "Compiler_Cfg_MCAL.h"*//* コメントアウト */
/*****************************************************************************/
/* Public macros                                                             */
/*****************************************************************************/

#define CDD_CODE
#define CDD_APPL_DATA
#define XCP_CODE
#define XCP_APPL_DATA
/* ---------------------------------------------------------------------------*/
/*                   RTE                                                    */
/* ---------------------------------------------------------------------------*/

#if (defined RTE_CODE) /* to prevent double definition */
#error RTE_CODE already defined
#endif /* if (defined RTE_CODE) */

/** \brief definition of the code memory class
 **
 ** To be used for code. */
#define RTE_CODE

#if (defined RTE_CONST) /* to prevent double definition */
#error RTE_CONST already defined
#endif /* if (defined RTE_CONST) */

/** \brief definition of the constant memory class
 **
 ** To be used for global or static constants. */
#define RTE_CONST

#if (defined RTE_APPL_DATA) /* to prevent double definition */
#error RTE_APPL_DATA already defined
#endif /* if (defined RTE_APPL_DATA) */

/** \brief definition of the application data pointer class
 **
 ** To be used for references on application data (expected to
 ** be in RAM or ROM) passed via API. */
#define RTE_APPL_DATA

#if (defined RTE_APPL_CONST) /* to prevent double definition */
#error RTE_APPL_CONST already defined
#endif /* if (defined RTE_APPL_CONST) */

/** \brief definition of the constant pointer class
 **
 ** To be used for references on application constants (expected to
 ** be certainly in ROM, for instance pointer of Init() function)
 ** passed via API. */
#define RTE_APPL_CONST

#if (defined RTE_APPL_CODE) /* to prevent double definition */
#error RTE_APPL_CODE already defined
#endif /* if (defined RTE_APPL_CODE) */

/** \brief definition of a code pointer class
 **
 ** To be used for references on application functions
 ** (e.g. call back function pointers). */
#define RTE_APPL_CODE

#if (defined RTE_VAR_NOINIT) /* to prevent double definition */
#error RTE_VAR_NOINIT already defined
#endif /* if (defined RTE_VAR_NOINIT) */

/** \brief definition of the noinit variable memory class
 **
 ** To be used for all global or static variables that are
 ** never initialized. */
#define RTE_VAR_NOINIT

#if (defined RTE_VAR_POWER_ON_INIT) /* to prevent double definition */
#error RTE_VAR_POWER_ON_INIT already defined
#endif /* if (defined RTE_VAR_POWER_ON_INIT) */

/** \brief definition of a power on init variable memory class
 **
 ** To be used for all global or static variables that are initialized
 ** only after power on reset. */
#define RTE_VAR_POWER_ON_INIT

#if (defined RTE_VAR_FAST) /* to prevent double definition */
#error RTE_VAR_FAST already defined
#endif /* if (defined RTE_VAR_FAST) */

/** \brief definition of a fast bariable memory class
 **
 ** To be used for all global or static variables that have at least one
 ** of the following properties:
 ** - accessed bitwise
 ** - frequently used
 ** - high number of accesses in source code */
#define RTE_VAR_FAST

#if (defined RTE_VAR) /* to prevent double definition */
#error RTE_VAR already defined
#endif /* if (defined RTE_VAR) */

/** \brief definition of a variable memory class
 **
 ** To be used for global or static variables that are initialized
 ** after every reset. */
#define RTE_VAR

/*================================== RAMTST ===================================*/

/* Memory type to be used for code */
#define RAMTST_CODE

/* Memory type to be used for Global or Static Constants */
#define RAMTST_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define RAMTST_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define RAMTST_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define RAMTST_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define RAMTST_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define RAMTST_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define RAMTST_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define RAMTST_VAR

/*================================== WDGIF ===================================*/

/* Memory type to be used for code */
#define WDGIF_CODE

/* Memory type to be used for Global or Static Constants */
#define WDGIF_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define WDGIF_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define WDGIF_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define WDGIF_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define WDGIF_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define WDGIF_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define WDGIF_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define WDGIF_VAR

/*=================================== ICU ====================================*/

/* Memory type to be used for code */
#define ICU_CODE

/* Memory type to be used for Global or Static Constants */
#define ICU_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define ICU_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define ICU_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define ICU_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define ICU_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define ICU_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define ICU_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define ICU_VAR


/*-------------------------- BSW Services-------------------------------------*/


/*================================== CANNM ===================================*/

/* Memory type to be used for code */
#define CANNM_CODE

/* Memory type to be used for Global or Static Constants */
#define CANNM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANNM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANNM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANNM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANNM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANNM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANNM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANNM_VAR

/* Memory type to be used for PB constants */
#define CANNM_CONST_PB

/*================================== CANSM ===================================*/

/* Memory type to be used for code */
#define CANSM_CODE

/* Memory type to be used for Global or Static Constants */
#define CANSM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANSM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANSM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANSM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANSM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANSM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANSM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANSM_VAR

/*================================== CANTP ===================================*/

/* Memory type to be used for code */
#define CANTP_CODE

/* Memory type to be used for Global or Static Constants */
#define CANTP_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANTP_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANTP_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANTP_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANTP_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANTP_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANTP_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANTP_VAR

/*=================================== COM ====================================*/

/* Memory type to be used for code */
#define COM_CODE

/* Memory type to be used for Global or Static Constants */
#define COM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define COM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define COM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define COM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define COM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define COM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define COM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define COM_VAR

#define COM_VAR_PB

#define COM_CONST_PB

#define COM_APPL_CONST_PB

#define COM_CONST_LT

#define COM_PDUR_CODE

#define COM_PDUR_APPL_DATA

#define COM_PDUR_CONST_PB

/*================================== COMM ====================================*/

/* Memory type to be used for code */
#define COMM_CODE
#define ComM_CODE
/* Memory type to be used for Global or Static Constants */
#define COMM_CONST
#define COMM_CONST_PB
/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define COMM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define COMM_APPL_CONST
#define COMM_APP_CONST
/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define COMM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define COMM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define COMM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define COMM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define COMM_VAR
#define COMM_VAR_PB

/*=================================== CRC ====================================*/

/* Memory type to be used for code */
#define CRC_CODE

/* Memory type to be used for Global or Static Constants */
#define CRC_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CRC_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CRC_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CRC_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CRC_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CRC_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CRC_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CRC_VAR

/*=================================== DCM ====================================*/

/* Memory type to be used for code */
#define DCM_CODE
#define Dcm_CODE        /* added for Rte genereated headers */

/* Memory type to be used for Global or Static Constants */
#define DCM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define DCM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define DCM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define DCM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define DCM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define DCM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define DCM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define DCM_VAR

#define DCM_APPL_VAR

/*=================================== DEM ====================================*/

/* Memory type to be used for code */
#define DEM_CODE
#define Dem_CODE       /* added for Rte genereated headers */

/* Memory type to be used for Global or Static Constants */
#define DEM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define DEM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define DEM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define DEM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define DEM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define DEM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define DEM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define DEM_VAR

#define DEM_NV_DATA

#define DEM_PBCFG_CONST

/*=================================== DET ====================================*/

/* Memory type to be used for code */
#define DET_CODE

/* Memory type to be used for Global or Static Constants */
#define DET_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define DET_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define DET_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define DET_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define DET_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define DET_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define DET_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define DET_VAR

/*================================== BSWM ====================================*/


#if (defined BSWM_CODE) /* to prevent double definition */
#error BSWM_CODE already defined
#endif /* if (defined CAN_CODE) */

/** \brief definition of the code memory class
 **
 ** To be used for code. */
#define BSWM_CODE
#define BswM_CODE        /* added for Rte genereated headers */

#if (defined BSWM_CONST) /* to prevent double definition */
#error BSWM_CONST already defined
#endif /* if (defined CAN_CONST) */

/** \brief definition of the constant memory class
 **
 ** To be used for global or static constants. */
#define BSWM_CONST

#if (defined BSWM_APPL_DATA) /* to prevent double definition */
#error BSWM_APPL_DATA already defined
#endif /* if (defined BSWM_APPL_DATA) */

/** \brief definition of the application data pointer class
 **
 ** To be used for references on application data (expected to
 ** be in RAM or ROM) passed via API. */
#define BSWM_APPL_DATA

#if (defined BSWM_APPL_CONST) /* to prevent double definition */
#error BSWM_APPL_CONST already defined
#endif /* if (defined CAN_APPL_CONST) */

/** \brief definition of the constant pointer class
 **
 ** To be used for references on application constants (expected to
 ** be certainly in ROM, for instance pointer of Init() function)
 ** passed via API. */
#define BSWM_APPL_CONST

#if (defined BSWM_CONFIG_CONST) /* to prevent double definition */
#error BSWM_CONFIG_CONST already defined
#endif /* if (defined CAN_APPL_CONST) */

/** \brief definition of the constant pointer class
 **
 ** To be used for references on application constants (expected to
 ** be certainly in ROM, for instance pointer of Init() function)
 ** passed via API. */
#define BSWM_CONFIG_CONST

#if (defined BSWM_APPL_CODE) /* to prevent double definition */
#error BSWM_APPL_CODE already defined
#endif /* if (defined CAN_APPL_CODE) */

/** \brief definition of a code pointer class
 **
 ** To be used for references on application functions
 ** (e.g. call back function pointers). */
#define BSWM_APPL_CODE

#define BswM_APPL_CODE

#if (defined BSWM_VAR_NOINIT) /* to prevent double definition */
#error BSWM_VAR_NOINIT already defined
#endif /* if (defined CAN_VAR_NOINIT) */


/** \brief definition of the noinit variable memory class
 **
 ** To be used for all global or static variables that are
 ** never initialized. */
#define BSWM_VAR_NOINIT

#if (defined BSWM_VAR_POWER_ON_INIT) /* to prevent double definition */
#error BSWM_VAR_POWER_ON_INIT already defined
#endif /* if (defined CAN_VAR_POWER_ON_INIT) */

/** \brief definition of a power on init variable memory class
 **
 ** To be used for all global or static variables that are initialized
 ** only after power on reset. */
#define BSWM_VAR_POWER_ON_INIT

#if (defined BSWM_VAR_FAST) /* to prevent double definition */
#error BSWM_VAR_FAST already defined
#endif /* if (defined CAN_VAR_FAST) */

/** \brief definition of a fast bariable memory class
 **
 ** To be used for all global or static variables that have at least one
 ** of the following properties:
 ** - accessed bitwise
 ** - frequently used
 ** - high number of accesses in source code */
#define BSWM_VAR_FAST

#if (defined BSWM_VAR) /* to prevent double definition */
#error BSWM_VAR already defined
#endif /* if (defined CAN_VAR) */

/** \brief definition of a variable memory class
 **
 ** To be used for global or static variables that are initialized
 ** after every reset. */
#define BSWM_VAR

#if (defined BSWM_NO_INIT_DATA) /* to prevent double definition */
#error BSWM_NO_INIT_DATA already defined
#endif /* if (defined CAN_VAR) */

/** \brief definition of a variable memory class
 **
 ** To be used for global or static variables that are initialized
 ** after every reset. */
#define BSWM_NO_INIT_DATA

/* ---------------------------------------------------------------------------*/
/*                   ECUM                                        */
/* ---------------------------------------------------------------------------*/

#if (defined ECUM_CODE) /* to prevent double definition */
#error ECUM_CODE already defined
#endif /* if (defined ECUM_CODE) */

/** \brief definition of the code memory class
 **
 ** To be used for code. */
#define ECUM_CODE
#define EcuM_CODE        /* added for Rte genereated headers */

#if (defined ECUM_CONST) /* to prevent double definition */
#error ECUM_CONST already defined
#endif /* if (defined ECUM_CONST) */

/** \brief definition of the constant memory class
 **
 ** To be used for global or static constants. */
#define ECUM_CONST

#if (defined ECUM_APPL_DATA) /* to prevent double definition */
#error ECUM_APPL_DATA already defined
#endif /* if (defined ECUM_APPL_DATA) */

/** \brief definition of the application data pointer class
 **
 ** To be used for references on application data (expected to
 ** be in RAM or ROM) passed via API. */
#define ECUM_APPL_DATA

#if (defined ECUM_APPL_CONST) /* to prevent double definition */
#error ECUM_APPL_CONST already defined
#endif /* if (defined ECUM_APPL_CONST) */

/** \brief definition of the constant pointer class
 **
 ** To be used for references on application constants (expected to
 ** be certainly in ROM, for instance pointer of Init() function)
 ** passed via API. */
#define ECUM_APPL_CONST

#if (defined ECUM_APPL_CODE) /* to prevent double definition */
#error ECUM_APPL_CODE already defined
#endif /* if (defined ECUM_APPL_CODE) */

/** \brief definition of a code pointer class
 **
 ** To be used for references on application functions
 ** (e.g. call back function pointers). */
#define ECUM_APPL_CODE

#if (defined ECUM_VAR_NOINIT) /* to prevent double definition */
#error ECUM_VAR_NOINIT already defined
#endif /* if (defined ECUM_VAR_NOINIT) */

/** \brief definition of the noinit variable memory class
 **
 ** To be used for all global or static variables that are
 ** never initialized. */
#define ECUM_VAR_NOINIT

#if (defined ECUM_VAR_POWER_ON_INIT) /* to prevent double definition */
#error ECUM_VAR_POWER_ON_INIT already defined
#endif /* if (defined ECUM_VAR_POWER_ON_INIT) */

/** \brief definition of a power on init variable memory class
 **
 ** To be used for all global or static variables that are initialized
 ** only after power on reset. */
#define ECUM_VAR_POWER_ON_INIT

#if (defined ECUM_VAR_FAST) /* to prevent double definition */
#error ECUM_VAR_FAST already defined
#endif /* if (defined ECUM_VAR_FAST) */

/** \brief definition of a fast bariable memory class
 **
 ** To be used for all global or static variables that have at least one
 ** of the following properties:
 ** - accessed bitwise
 ** - frequently used
 ** - high number of accesses in source code */
#define ECUM_VAR_FAST

#if (defined ECUM_VAR) /* to prevent double definition */
#error ECUM_VAR already defined
#endif /* if (defined ECUM_VAR) */

/** \brief definition of a variable memory class
 **
 ** To be used for global or static variables that are initialized
 ** after every reset. */
#define ECUM_VAR

/*=================================== Fee ====================================*/

#define FEE_PUBLIC_CODE                /* API functions                       */
#define FEE_PUBLIC_CONST               /* API constants                       */

#define FEE_PRIVATE_CODE               /* Internal functions                  */

#define FEE_PRIVATE_DATA               /* Module internal data                */
#define FEE_PRIVATE_CONST              /* Internal ROM Data                   */

#define FEE_APPL_CODE                  /* callbacks of the Application        */
#define FEE_APPL_CONST                 /* Applications' ROM Data              */
#define FEE_APPL_DATA                  /* Applications' RAM Data              */
#define FEE_FAST_DATA                  /* 'Near' RAM Data                     */


#define FEE_CONFIG_CONST               /* Desc. Tables -> Config-dependent    */
#define FEE_CONFIG_DATA                /* Config. dependent (reg. size) data  */

#define FEE_INIT_DATA                  /* Data which is initialized during
                                          Startup                             */
#define FEE_NOINIT_DATA                /* Data which is not initialized during
                                          Startup                             */
#define FEE_CONST                      /* Data Constants                      */


/*=================================== FIM ====================================*/

/* Memory type to be used for code */
#define FIM_CODE
#define FiM_CODE        /* added for Rte genereated headers */

/* Memory type to be used for Global or Static Constants */
#define FIM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FIM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FIM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FIM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FIM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FIM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FIM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FIM_VAR

/* to support Fim module build */
#define FIM_PBCFG_CONST

/*================================== FRNM ====================================*/

/* Memory type to be used for code */
#define FRNM_CODE

/* Memory type to be used for Global or Static Constants */
#define FRNM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FRNM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FRNM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FRNM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FRNM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FRNM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FRNM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FRNM_VAR

/*================================== FRSM ====================================*/

/* Memory type to be used for code */
#define FRSM_CODE

/* Memory type to be used for Global or Static Constants */
#define FRSM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FRSM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FRSM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FRSM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FRSM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FRSM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FRSM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FRSM_VAR

/*================================== FRTP ====================================*/

/* Memory type to be used for code */
#define FRTP_CODE

/* Memory type to be used for Global or Static Constants */
#define FRTP_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FRTP_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FRTP_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FRTP_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FRTP_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FRTP_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FRTP_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FRTP_VAR

/*================================== IPDUM ===================================*/

/* Memory type to be used for code */
#define IPDUM_CODE

/* Memory type to be used for Global or Static Constants */
#define IPDUM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define IPDUM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define IPDUM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define IPDUM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define IPDUM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define IPDUM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define IPDUM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define IPDUM_VAR

/*================================== LINNM ===================================*/

/* Memory type to be used for code */
#define LINNM_CODE

/* Memory type to be used for Global or Static Constants */
#define LINNM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define LINNM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define LINNM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define LINNM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define LINNM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define LINNM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define LINNM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define LINNM_VAR

/*================================== LINSM ===================================*/

/* Memory type to be used for code */
#define LINSM_CODE

/* Memory type to be used for Global or Static Constants */
#define LINSM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define LINSM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define LINSM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define LINSM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define LINSM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define LINSM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define LINSM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define LINSM_VAR

/*================================== MEMIF ===================================*/

/* Memory type to be used for code */
#define MEMIF_CODE

/* Memory type to be used for Global or Static Constants */
#define MEMIF_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define MEMIF_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define MEMIF_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define MEMIF_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define MEMIF_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define MEMIF_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define MEMIF_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define MEMIF_VAR

/*=================================== NM =====================================*/

/* Memory type to be used for code */
#define NM_CODE

/* Memory type to be used for Global or Static Constants */
#define NM_CONST
#define NM_CONST_PB

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define NM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define NM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define NM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define NM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define NM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define NM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define NM_VAR

/*=================================== NVM ====================================*/

/* Memory type to be used for code */
#define NVM_CODE
#define NvM_CODE        /* added for Rte genereated headers */

/* Memory type to be used for Global or Static Constants */
#define NVM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define NVM_APPL_DATA
#define NvM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define NVM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define NVM_APPL_CODE
#define NvM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define NVM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define NVM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define NVM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define NVM_VAR

/*================================== OS ====================================*/

/*
 * Memory type to be used for code
 */
#define OS_CODE

/*
 * Memory type to be used for global or static Constants
 */
#define OS_CONST

/*
 * Memory type to be used for the reference on application data (expected to be
 * in RAM or ROM) passed via API
 */
#define OS_APPL_DATA

/*
 * Memory type to be used for the reference on application constants(expected
 * to be certainly in ROM, for instance pointer of init function) passed via
 * API */
#define OS_APPL_CONST

/*
 * Memory type to be used for the reference on application functions (e.g. call
 * back function pointers)
 */
#define OS_APPL_CODE

/*
 * Memory type to be used for references on application
 * functions. (e.g. callout function pointers)
 */
#define OS_CALLOUT_CODE

/*
 * Memory type to be used for all global or static variables that are never
 * initialized
 */
#define OS_VAR_NOINIT

/*
 * Memory type to be used for all global or static variables that are
 * initialized only after power on reset
 */
#define OS_VAR_POWER_ON_INIT

/*
 * Memory type to be used for all global or static variables that have at least
 * one of the following properties:
 *   1. accessed bitwise
 *   2. frequently used
 *   3. high number of access in source code
 */
#define OS_VAR_FAST

/*
 * Memory type to be used for all global or static variables that are
 * initialized after every reset
 */
#define OS_VAR

/*================================== PDUR ====================================*/

/* Memory type to be used for code */
#define PDUR_CODE

/* Memory type to be used for Global or Static Constants */
#define PDUR_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define PDUR_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define PDUR_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define PDUR_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define PDUR_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define PDUR_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define PDUR_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define PDUR_VAR

/* Memory type to be used for PB variables */
#define PDUR_VAR_PB

/* Memory type to be used for PB constants */
#define PDUR_CONST_PB



/*================================== SCHM ====================================*/

/* Memory type to be used for code */
#define SCHM_CODE

/* Memory type to be used for Global or Static Constants */
#define SCHM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define SCHM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define SCHM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define SCHM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define SCHM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define SCHM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define SCHM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define SCHM_VAR

/*================================== WDGM ====================================*/

/* Memory type to be used for code */
#define WDGM_CODE
#define WdgM_CODE

/* Memory type to be used for Global or Static Constants */
#define WDGM_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define WDGM_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define WDGM_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define WDGM_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define WDGM_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define WDGM_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define WDGM_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define WDGM_VAR


/*--------------------- BSW ECU Abstraction ----------------------------------*/

#define ECU_APPL_DATA

/*================================== CANIF ===================================*/

/* Memory type to be used for code */
#define CANIF_CODE

/* Memory type to be used for Global or Static Constants */
#define CANIF_CONST

#define CANIF_CONST_PB

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANIF_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANIF_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANIF_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANIF_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANIF_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANIF_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANIF_VAR
#define CANIF_VAR_PB

/*================================= CANTRCV ==================================*/

/* Memory type to be used for code */
#define CANTRCV_CODE

/* Memory type to be used for Global or Static Constants */
#define CANTRCV_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANTRCV_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANTRCV_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANTRCV_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANTRCV_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANTRCV_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANTRCV_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANTRCV_VAR

#define CANTRCV_PUBLIC_CODE

/*=================================== EA =====================================*/

/* Memory type to be used for code */
#define EA_CODE

/* Memory type to be used for Global or Static Constants */
#define EA_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define EA_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define EA_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define EA_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define EA_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define EA_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define EA_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define EA_VAR

/*================================== FRIF ====================================*/

/* Memory type to be used for code */
#define FRIF_CODE

/* Memory type to be used for Global or Static Constants */
#define FRIF_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FRIF_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FRIF_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FRIF_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FRIF_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FRIF_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FRIF_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FRIF_VAR
#define FRIF_VAR_PB

#define FRIF_CONST_PB

#define FRIF_FRTRCV_APPL_DATA
#define FRIF_FR_CONST

/*================================= FRTRCV ===================================*/

#define FRTRCV_CALLOUT_CODE

/* Memory type to be used for code */
#define FRTRCV_CODE

/* Memory type to be used for Global or Static Constants */
#define FRTRCV_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define FRTRCV_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define FRTRCV_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define FRTRCV_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define FRTRCV_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define FRTRCV_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define FRTRCV_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define FRTRCV_VAR
/*================================= CANTRCV TJA1041 ==========================*/

/* Memory type to be used for code */
#define CANTRCV_31_TJA1041_CODE

/* Memory type to be used for Global or Static Constants */
#define CANTRCV_31_TJA1041_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANTRCV_31_TJA1041_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANTRCV_31_TJA1041_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANTRCV_31_TJA1041_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANTRCV_31_TJA1041_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANTRCV_31_TJA1041_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANTRCV_31_TJA1041_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANTRCV_31_TJA1041_VAR

#define CANTRCV_31_TJA1041_PUBLIC_CODE
/*================================= CANTRCV TJA1041 ==========================*/

/* Memory type to be used for code */
#define CANTRCV_31_TJA1051_CODE

/* Memory type to be used for Global or Static Constants */
#define CANTRCV_31_TJA1051_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define CANTRCV_31_TJA1051_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define CANTRCV_31_TJA1051_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define CANTRCV_31_TJA1051_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define CANTRCV_31_TJA1051_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define CANTRCV_31_TJA1051_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define CANTRCV_31_TJA1051_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define CANTRCV_31_TJA1051_VAR

#define CANTRCV_31_TJA1051_PUBLIC_CODE
/*================================= FRTRCV TJA1080 ===================================*/

#define FRTRCV_31_TJA1080_CONST

/*================================= IOHWAB ===================================*/

/* Memory type to be used for code */
#define IOHWAB_CODE

/* Memory type to be used for Global or Static Constants */
#define IOHWAB_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define IOHWAB_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define IOHWAB_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define IOHWAB_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define IOHWAB_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define IOHWAB_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define IOHWAB_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define IOHWAB_VAR

/*================================== LINIF ===================================*/

/* Memory type to be used for code */
#define LINIF_CODE

/* Memory type to be used for Global or Static Constants */
#define LINIF_CONST

/* Memory type to be used for the reference on application data (expected to be
   in RAM or ROM) passed via API */
#define LINIF_APPL_DATA

/* Memory type to be used for the reference on application constants(expected
   to be certainly in ROM, for instance pointer of init function) passed via
   API */
#define LINIF_APPL_CONST

/* Memory type to be used for the reference on application functions (e.g. call
   back function pointers) */
#define LINIF_APPL_CODE

/* Memory type to be used for all global or static variables that are never
   initialized */
#define LINIF_VAR_NOINIT

/* Memory type to be used for all global or static variables that are
   initialized only after power on reset */
#define LINIF_VAR_POWER_ON_INIT

/* Memory type to be used for all global or static variables that have at least
   one of the following properties
   1. accessed bitwise
   2. frequently used
   3. High number of access in source code */
#define LINIF_VAR_FAST

/* Memory type to be used for all global or static variables that are
   initialized after every reset */
#define LINIF_VAR

/*=============================== SWC ===================================*/
#if (defined LED01_CODE) /* to prevent double definition */
#error LED01_CODE already defined
#endif /* if (defined LED01_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define LED01_CODE

#if (defined Agent_01_CODE) /* to prevent double definition */
#error Agent_01_CODE already defined
#endif /* if (defined Agent_01_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define Agent_01_CODE

#if (defined app_func_skeycon_bz_CODE) /* to prevent double definition */
#error app_func_skeycon_bz_CODE already defined
#endif /* if (defined app_func_skeycon_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_func_skeycon_bz_CODE

#if (defined app_func_atrev_bz_CODE) /* to prevent double definition */
#error app_func_atrev_bz_CODE already defined
#endif /* if (defined app_func_atrev_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_func_atrev_bz_CODE

#if (defined app_func_skeydcon2_bz_CODE) /* to prevent double definition */
#error app_func_skeydcon2_bz_CODE already defined
#endif /* if (defined app_func_skeydcon2_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_func_skeydcon2_bz_CODE

#if (defined app_func_skeydcon_bz_CODE) /* to prevent double definition */
#error app_func_skeydcon_bz_CODE already defined
#endif /* if (defined app_func_skeydcon_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_func_skeydcon_bz_CODE

#if (defined app_fw_timer_CODE) /* to prevent double definition */
#error app_fw_timer_CODE already defined
#endif /* if (defined app_fw_timer_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_fw_timer_CODE

#if (defined app_in_shiftr1_CODE) /* to prevent double definition */
#error app_in_shiftr1_CODE already defined
#endif /* if (defined app_in_shiftr1_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_shiftr1_CODE

#if (defined app_in_shiftr2_CODE) /* to prevent double definition */
#error app_in_shiftr2_CODE already defined
#endif /* if (defined app_in_shiftr2_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_shiftr2_CODE

#if (defined app_in_smtkey_cont_CODE) /* to prevent double definition */
#error app_in_smtkey_cont_CODE already defined
#endif /* if (defined app_in_smtkey_cont_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_smtkey_cont_CODE

#if (defined app_in_smtkey_intr_CODE) /* to prevent double definition */
#error app_in_smtkey_intr_CODE already defined
#endif /* if (defined app_in_smtkey_intr_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_smtkey_intr_CODE

#if (defined app_in_smtkey_intr2_CODE) /* to prevent double definition */
#error app_in_smtkey_intr2_CODE already defined
#endif /* if (defined app_in_smtkey_intr2_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_smtkey_intr2_CODE

#if (defined app_out_bz_CODE) /* to prevent double definition */
#error app_out_bz_CODE already defined
#endif /* if (defined app_out_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_out_bz_CODE

#if (defined app_outc_bz_CODE) /* to prevent double definition */
#error app_outc_bz_CODE already defined
#endif /* if (defined app_outc_bz_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_outc_bz_CODE

#if (defined app_sleep_ctrl_CODE) /* to prevent double definition */
#error app_sleep_ctrl_CODE already defined
#endif /* if (defined app_sleep_ctrl_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_sleep_ctrl_CODE

#if (defined Cd_Sgdrv_CODE) /* to prevent double definition */
#error Cd_Sgdrv_CODE already defined
#endif /* if (defined Cd_Sgdrv_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define Cd_Sgdrv_CODE

#if (defined app_in_igsts_CODE) /* to prevent double definition */
#error app_in_igsts_CODE already defined
#endif /* if (defined app_in_igsts_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define app_in_igsts_CODE

#if (defined Iohawb_IgSts_CODE) /* to prevent double definition */
#error Iohawb_IgSts_CODE already defined
#endif /* if (defined Iohawb_IgSts_CODE) */

/** \brief definition of the code memory class **
 ** To be used for code. */
#define Iohawb_IgSts_CODE


#define LED01_CODE

#define Agent_01_CODE

#define app_func_skeycon_bz_CODE

#define app_func_atrev_bz_CODE

#define app_func_skeydcon2_bz_CODE

#define app_func_skeydcon_bz_CODE

#define app_fw_timer_CODE

#define app_in_shiftr1_CODE

#define app_in_shiftr2_CODE

#define app_in_smtkey_cont_CODE

#define app_in_smtkey_intr_CODE

#define app_in_smtkey_intr2_CODE

#define app_out_bz_CODE

#define app_outc_bz_CODE

#define app_sleep_ctrl_CODE

#define Cd_Sgdrv_CODE

#define app_in_igsts_CODE

#define Iohawb_IgSts_CODE

/* #define SnrActSwC_ModeChangeISR_CODE */

/*****************************************************************************/
/* Public types                                                              */
/*****************************************************************************/

/*****************************************************************************/
/* Public constant & variable prototypes                                     */
/*****************************************************************************/

/*****************************************************************************/
/* Public API function prototypes                                            */
/*****************************************************************************/

#endif /* COMPILER_CFG_H */

