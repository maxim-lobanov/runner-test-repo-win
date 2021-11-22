.. # Links
.. _CPython: http://www.python.org/
.. _PyPy: http://www.pypy.org/
.. _PyPi: http://pypi.org/
.. _repository: https://github.com/rastern/umsg
.. _releases: https://github.com/rastern/umsg/releases
.. _LGPL3: https://www.gnu.org/licenses/lgpl-3.0.en.html


===============
Getting Started
===============

About
=====

The *umsg* package is intended as an aid for library logging with some enhancements
upon Python's native :py:mod:`logging` library. The name derives from a universally
named function, _msg(), I've used across projects and languages historically.
The underscore was changed to a `u` to make the name PEP8 compliant, and the
preferred pronunciation is 'you message' or (ju ˈmɛsəʤ) in IPA.



Installation
============

.. code-block:: bash

   pip3 install umsg

Requirements
============

*umsg* is designed to work with Python 3.5 and higher. The package has been
tested against the following interpreters

- CPython_ 3.5.7
- CPython_ 3.6.10
- CPython_ 3.7.6
- CPython_ 3.8.0
- CPython_ 3.9.0a3+ (dev)
- PyPy_ 3.6.9

Usage
=====

*umsg* works "out of the box", without any configuration.

.. code-block:: python

  import umsg

  umsg.log('hello logging')

However, because it is intended for libraries, you may find it more useful to
set a handler other than the default :py:class:`~logging.NullHandler`.

.. code-block:: python

  import logging
  import umsg

  umsg.add_handler(logging.StreamHandler())
  umsg.log('send me to stdout!')


GitHub Source
=============

The source code for *umsg* is availble on GitHub repository_.

Individual release archives may be found `here`__.

__ releases_

Author
======

*umsg* is written and maintained by R.A. Stern.


License
=======

*umsg* is distributed under the `GNU Lesser General Public License <https://www.gnu.org/licenses/lgpl-3.0.en.html>`_ software license, which may
also be obtained from the GNU project, https://www.gnu.org/.
