==============================================
_msg: Python Library Logging Facility
==============================================

_msg, pronounced 'you-message', is a module level logging utility intended for
libraries, which works just as well for normal logging scenarios.
The package name replaces the '_' with a 'u' to conform with PEP8 standards.


Installation
============

.. code:: bash

  pip install umsg


Usage
=====

Class Logging Made Easy
-----------------------

.. code:: python

  from umsg import LoggingMixin

  class MyClass(LoggingMixin):
      def __init__(self):
          super().__init__(prefix='MyClass')
          self.log('Logging initiated', level='debug')


Basic Module / Script Logging Too
---------------------------------

.. code:: python

  import logging
  import umsg

  umsg.add_handler(logging.StreamHandler())
  umsg.log('Good here')


Documentation
=============

Detailed documentation is available on `readthedocs <https://umsg.readthedocs.io/en/latest/>`_.
