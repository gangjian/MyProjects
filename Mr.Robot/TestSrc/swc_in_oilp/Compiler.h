/******************************************************************************
 *
 *                    Mentor Graphics Corporation
 *                        All rights reserved
 *
 ******************************************************************************
 *
 * Module:       Common - Compiler Abstraction
 *
 * File Name:    Compiler.h
 *
 * Description:  This file contains the definitions and macros specified by
 *               AUTOSAR for the abstraction of compiler specific keywords.
 *
 *
 *
 ******************************************************************************/

/*COMPILER047*/
#ifndef COMPILER_H
#define COMPILER_H

#ifdef __cplusplus
extern "C"
{
#endif

/*=============================== INCLUSIONS =================================*/

/*COMPILER052*/
#include "Compiler_Cfg.h"

/*============================== COMPILER INFO ===============================*/

/**
 * \defgroup Compiler Abstraction
 *  Describes the compiler information
 */

/* Compiler vendor id */
#define COMPILER_VENDOR_ID           ((uint16) 31)

/* Compiler Autosar Specification major version */
#define COMPILER_AR_RELEASE_MAJOR_VERSION (4u)

/* Compiler Autosar Specification minor version */
#define COMPILER_AR_RELEASE_MINOR_VERSION (0u)

/* Compiler Autosar Specification patch version */
#define COMPILER_AR_RELEASE_REVISION_VERSION (2u)

/* Compiler Software specification major version */
#define COMPILER_SW_MAJOR_VERSION (1u)

/* Compiler Software specification minor version */
#define COMPILER_SW_MINOR_VERSION (0u)

/* Compiler Software specification patch version */
#define COMPILER_SW_PATCH_VERSION (0u)

/* COMPILER010 */
/* PRQA S 0602 1 *//* The identifier is reserved for use by the library. - Scheme is defined by AUTOSAR */
#define _GREENHILLS_C_V850_

/*============================ COMPILER KEYWORDS =============================*/

/**
 * \defgroup Compiler Keywords
 */
#define STATIC  static

/* COMPILER046 */
/* The memory class is used for the declaration of local pointers */
#define AUTOMATIC

/** COMPILER059 */
/* The memory class is used within type definitions, where no memory
   qualifier can be specified */
#define TYPEDEF

/* This is used to define the abstraction of compiler keyword static inline*/
#define LOCAL_INLINE      static inline

/** COMPILER051 */
/* This is used to define the void pointer to zero definition. */
#define NULL_PTR    ((void *)0)

/*COMPILER057: */
/* This is used to define the abstraction of compiler keyword inline */
/* inline behavior (exported/imported) differs according to specified dialect GNU / C89/C99 */
#define INLINE    inline

#define _INTERRUPT_ /* PRQA S 0602*/__interrupt

/* General hints for target V850E2V3 GHS:
   Available target keywords for
       memclass of functions: __nearcall __farcall

   Available target type qualifiers for
       memclass of constants, variables and
       ptrclass of pointers: __bytereversed, __bigendian,
                             __littleendian, __packed

   Available target type qualifiers for
       memclass of pointers:  - */

/**
 * \brief  This macro is used for the declaration and definition of
 *         functions, that ensures correct syntax of function declarations as
 *         required by a specific compiler
 *
 * \param  rettype   return type of the function.
 * \param  memclass  classification of the function itself
 *
 * \retval None
 *
 */
/* PRQA S 3453 ++ */  /*not complying with MISRA Rule 19.7 to fulfill
                     the compiler abstraction SWS document */
/* PRQA S 3410 ++ */ /*not complying with MISRA Rule 19.10 to fulfill
                     the compiler abstraction SWS document */
/** COMPILER001 */
#define FUNC(rettype, memclass) memclass rettype

/** BUGFIX for bug #4953 on Joint Bugzilla */
#define FUNC_P2CONST(ptrtype, ptrclass, memclass) memclass const ptrclass ptrtype *


/**
 * \brief  This macro is used for the declaration and definition of pointers
 *         in RAM, pointing to variables
 *
 * \param  ptrtype   type of the reference variable
 * \param  memclass  classification of the pointer's variable itself
 * \param  ptrclass  defines the classification of the pointer distance
 *
 * \retval None
 *
 */
/** COMPILER006 */
#define P2VAR(ptrtype, memclass, ptrclass) /* PRQA S 3409 */ptrclass ptrtype *

/**
 * \brief  This macro is used for the declaration and definition of pointers
 *         in RAM, pointing to constants
 *
 * \param  ptrtype   type of the reference variable.
 * \param  memclass  classification of the pointer's variable itself
 * \param  ptrclass  defines the classification of the pointer distance
 *
 * \retval None
 *
 */
/** COMPILER013 */
#define P2CONST(ptrtype, memclass, ptrclass) /* PRQA S 3409 */ const ptrclass ptrtype *


/**
 * \brief  This macro is used for the declaration and definition of constant
 *         pointers accessing variables
 *
 * \param  ptrtype   type of the reference variable
 * \param  memclass  classification of the pointer's constant itself
 * \param  ptrclass  defines the classification of the pointer distance
 *
 * \retval None
 *
 */
/** COMPILER031 */
#define CONSTP2VAR(ptrtype, memclass, ptrclass) /* PRQA S 3409 */ ptrclass ptrtype * const

/**
 * \brief  This macro is used for the declaration and definition of constant
 *         pointers accessing variables
 *
 * \param  ptrtype   type of the reference variable.
 * \param  memclass  classification of the pointer's constant itself
 * \param  ptrclass  defines the classification of the pointer distance
 *
 * \retval None
 *
 */
/** COMPILER032 */
#define CONSTP2CONST(ptrtype, memclass, ptrclass) /* PRQA S 3409 */ const ptrclass ptrtype * const


/**
 * \brief  This macro is used for the declaration and definition of constant
 *         pointers accessing variables
 *
 * \param  rettype   return type of the function
 * \param  ptrclass  defines the classification of the pointer's distance
 * \param  fctname   classification of the pointer's variable itself
 *
 * \retval None
 *
 */
/** COMPILER039 */
#define P2FUNC(rettype, ptrclass, fctname) rettype (*fctname)

/* This is used to for the declaration and definition of constants */
/** COMPILER023 */
#define CONST(type, memclass) /* PRQA S 3409 */ const memclass type

/* This is used to for the declaration and definition of variables */
/** COMPILER026 */
#define VAR(type, memclass) memclass type

/* PRQA S 3453 -- */
/* PRQA S 3410 -- */

/* Inline assembler support for HALT instruction */ 
#define ASM_HALT() __asm("halt")

/* Inline assembler support for NOP instruction */ 
#define ASM_NOP() __asm("nop")

#ifdef __cplusplus
}
#endif

#endif /* COMPILER_H */
